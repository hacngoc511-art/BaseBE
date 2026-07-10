# Recommend Guide

Tài liệu này liệt kê các pattern/kỹ thuật khác có thể áp dụng thêm vào project để phát triển xa hơn feature Student mẫu hiện tại. Đây là gợi ý cho bước tiếp theo, không phải mô tả code đã có sẵn.

## 1. Validation Pattern (FluentValidation)
Hiện tại `CreateStudentCommand`, `UpdateStudentCommand` không có bước validate input (age âm, email sai định dạng, fullName rỗng...). Model binding của ASP.NET Core chỉ đảm bảo đúng kiểu dữ liệu, không đảm bảo đúng nghiệp vụ.

Gợi ý:
- Thêm package `FluentValidation.AspNetCore`.
- Tạo `CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>` trong Application layer.
- Đăng ký validator trong `Program.cs`, chạy validate trước khi vào handler (qua middleware hoặc filter).

Lợi ích: tách rule validate ra khỏi handler, dễ test riêng, dễ tái sử dụng rule giữa Create và Update.

## 2. Result Pattern (thay thế trả `null`/`bool`)
`StudentCommandHandler.UpdateAsync` trả `StudentDto?`, `DeleteAsync` trả `bool` để controller tự suy ra 404. Cách này đơn giản nhưng không phân biệt được lý do thất bại (không tìm thấy vs. vi phạm rule nghiệp vụ vs. lỗi khác).

Gợi ý: tạo `Result<T>` hoặc `Result` record chứa `IsSuccess`, `Error`, `Value`, để handler trả về lý do thất bại rõ ràng, controller map sang status code chính xác hơn (404 vs 409 vs 400) mà vẫn không cần throw exception cho luồng nghiệp vụ bình thường.

## 3. Global Exception Handling Middleware
Hiện tại project chưa có middleware xử lý exception tập trung. Nếu EF Core throw `DbUpdateException` (ví dụ trùng email) hoặc bug bất ngờ, client sẽ nhận response lỗi mặc định của ASP.NET Core.

Gợi ý: thêm middleware (`app.UseExceptionHandler(...)` hoặc custom middleware) trả về response dạng chuẩn `ProblemDetails` (RFC 7807), log lỗi tập trung, tránh lộ stack trace ra client ở môi trường Production.

## 4. Pagination cho Query danh sách
`GetStudentsQuery`/`GetAllAsync` hiện trả toàn bộ danh sách, sẽ có vấn đề hiệu năng khi dữ liệu lớn.

Gợi ý:
- Mở rộng `GetStudentsQuery(int Page = 1, int PageSize = 20)`.
- Repository dùng `Skip`/`Take` trên `IQueryable<Student>`.
- Trả về kiểu `PagedResult<StudentDto>` gồm `Items`, `TotalCount`, `Page`, `PageSize`.

## 5. AutoMapper hoặc mapping tay có tổ chức
Việc map `Student` → `StudentDto` hiện làm thủ công trong từng handler (`new StudentDto(s.Id, s.FullName, ...)`). Khi entity có nhiều field hoặc nhiều DTO khác nhau (list DTO, detail DTO), map tay dễ lặp code và dễ quên field.

Gợi ý: dùng `AutoMapper` với `Profile` định nghĩa map rule một chỗ, hoặc ít nhất tạo static extension method `ToDto()` để tập trung logic mapping, tránh lặp ở nhiều handler.

## 6. Specification Pattern
Khi cần filter/search phức tạp hơn (ví dụ tìm student theo tên, theo khoảng tuổi, theo domain email), thay vì thêm nhiều method vào `IStudentRepository` (`GetByNameAsync`, `GetByAgeRangeAsync`...), có thể dùng Specification Pattern: định nghĩa `ISpecification<Student>` chứa biểu thức filter, repository có một method `ListAsync(ISpecification<Student> spec)` duy nhất, tái sử dụng được cho mọi tiêu chí lọc.

## 7. Soft Delete
`DeleteAsync` hiện xoá cứng record khỏi database (`_context.Students.Remove(entity)`). Với dữ liệu quan trọng, nên cân nhắc soft delete: thêm field `IsDeleted`/`DeletedAt` vào entity, `DeleteAsync` chỉ set field đó, và các query mặc định filter `IsDeleted == false` (dùng EF Core Global Query Filter).

## 8. Domain Events / Aggregate Root (khi nghiệp vụ phức tạp hơn)
Hiện `Student` là entity đơn giản, không có domain event. Khi nghiệp vụ phát triển (ví dụ tạo student cần gửi email chào mừng, cập nhật student cần ghi audit log), nên cân nhắc:
- Domain Events: entity phát ra event (`StudentCreatedEvent`) khi thay đổi trạng thái, handler khác lắng nghe để xử lý side-effect mà không làm phình to `StudentCommandHandler`.
- Aggregate Root: nếu sau này Student có quan hệ với entity con (ví dụ `Enrollment`), cần xác định rõ aggregate boundary để đảm bảo tính nhất quán khi save.

## 9. Options Pattern cho cấu hình
Nếu có thêm cấu hình ngoài connection string (ví dụ giới hạn page size, feature flag), nên dùng Options Pattern (`IOptions<T>`) thay vì đọc trực tiếp `IConfiguration` rải rác trong code, giúp cấu hình có kiểu dữ liệu rõ ràng và dễ test.

## 10. Caching (Decorator Pattern quanh Repository)
Với query đọc nhiều (`GetAllAsync`), có thể áp dụng Decorator Pattern: tạo `CachedStudentRepository : IStudentRepository` bọc quanh `StudentRepository`, cache kết quả bằng `IMemoryCache`/Redis, mà không cần sửa gì ở Application layer vì vẫn tuân theo interface `IStudentRepository` sẵn có.

## 11. API Versioning
Khi API đã có client sử dụng thật, nên thêm `Asp.Versioning.Mvc` để version hoá endpoint (`/api/v1/students`), tránh breaking change khi cần đổi contract.

## 12. Unit Test / Integration Test cho từng layer
Project hiện chưa có test project. Gợi ý cấu trúc test theo đúng layer đã tách sẵn:
- Unit test cho `StudentCommandHandler`/`StudentQueryHandler` bằng cách mock `IUnitOfWork`/`IStudentRepository` (Moq hoặc NSubstitute) — vì Application layer không phụ thuộc EF Core nên mock rất dễ.
- Integration test cho `StudentRepository` dùng EF Core InMemory provider hoặc Testcontainers SQL Server, để đảm bảo query thật đúng.
- Test controller bằng `WebApplicationFactory<Program>` để test end-to-end qua HTTP.

## 13. Thứ tự ưu tiên khi áp dụng
Nếu chỉ chọn vài mục để làm trước, nên ưu tiên theo thứ tự:
1. Validation Pattern — vì hiện tại chưa có bất kỳ validate nào, rủi ro cao nhất.
2. Unit Test cho handler — bảo vệ logic CRUD vừa hoàn thiện.
3. Global Exception Handling Middleware — tránh lộ lỗi hệ thống ra client.
4. Pagination — cần thiết ngay khi dữ liệu Student thật sự lớn dần.
5. Các mục còn lại tuỳ theo nhu cầu nghiệp vụ phát sinh.
