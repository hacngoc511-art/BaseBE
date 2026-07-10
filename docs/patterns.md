# Patterns Guide

## 1. Repository Pattern
Repository Pattern giúp tách logic truy cập dữ liệu khỏi business logic.

Trong dự án này:
- Interface repository nằm trong Domain, ví dụ IStudentRepository.
- Implementation nằm trong Infrastructure, ví dụ StudentRepository.

Lợi ích:
- Controller và handler không cần biết database đang dùng SQL Server hay PostgreSQL.
- Dễ thay đổi storage layer mà không ảnh hưởng nhiều tới business code.
- Dễ test bằng cách mock repository.

Ví dụ thực tế trong project:
- StudentCommandHandler không tự query database trực tiếp.
- Nó sử dụng IUnitOfWork để thao tác dữ liệu ở mức abstraction.

## 2. Unit of Work Pattern
Unit of Work giúp gom nhiều thao tác persistence lại thành một đơn vị làm việc.

Trong project hiện tại:
- IUnitOfWork định nghĩa các repository cần dùng và method SaveChangesAsync.
- UnitOfWork ở Infrastructure triển khai các repository và quản lý transaction-like behavior.

Lợi ích:
- Bạn có thể thực hiện nhiều thao tác liên quan đến cùng một unit logic rồi lưu một lần.
- Giữ dữ liệu nhất quán hơn.
- Làm code dễ mở rộng khi feature cần nhiều repository cùng lúc.

## 3. Factory Pattern
Factory Pattern dùng để tạo object mà không để caller biết cách khởi tạo chi tiết.

Trong project:
- StudentFactory nằm trong Infrastructure.
- Nếu sau này có logic tạo student theo nhiều rule khác nhau, factory sẽ tập trung hóa việc khởi tạo đó.

Lợi ích:
- Giảm độ phức tạp ở code gọi.
- Dễ thay đổi quy trình tạo object mà không sửa nhiều nơi.

## 4. CQRS Pattern
CQRS là viết tắt của Command Query Responsibility Segregation, nghĩa là phân tách rõ ràng giữa thao tác ghi và thao tác đọc.

Trong project này:
- Command dùng cho thao tác thay đổi dữ liệu, ví dụ CreateStudentCommand.
- Query dùng cho thao tác đọc dữ liệu, ví dụ GetStudentsQuery.
- Handler riêng cho từng loại: StudentCommandHandler và StudentQueryHandler.

Lợi ích:
- Code dễ đọc hơn vì mỗi handler chỉ làm một trách nhiệm.
- Dễ mở rộng khi một use case cần logic đọc và ghi khác nhau.
- Phù hợp cho nền tảng có nhiều nghiệp vụ phức tạp.

## 5. Mối quan hệ giữa các pattern trong dự án
Các pattern ở đây không tách rời nhau mà bổ trợ cho nhau:
- CQRS giúp phân tách request thành command/query.
- Repository và Unit of Work giúp abstraction cho dữ liệu.
- Factory giúp tạo object theo cách chuẩn hóa.

Khi đọc code, bạn nên nhìn thấy sự kết hợp này như một hệ thống thống nhất thay vì các pattern riêng lẻ.

## 6. CRUD Pattern đầy đủ (Create - Read - Update - Delete)
Feature Student trong project minh họa một bộ CRUD hoàn chỉnh, đi xuyên suốt cả 4 layer. Đây là "khuôn mẫu" nên copy khi thêm entity mới.

### 6.1 Create
- API: `POST /api/students`, nhận body là `CreateStudentCommand(FullName, Age, Email)`.
- Handler: `StudentCommandHandler.CreateAsync` tạo entity `Student` mới, gọi `IUnitOfWork.Students.AddAsync` rồi `SaveChangesAsync`.
- Response: `201 Created` kèm `StudentDto` và header `Location` trỏ tới `GetById`.

### 6.2 Read (danh sách + theo id)
- Đọc danh sách: `GET /api/students` dùng `GetStudentsQuery` (query rỗng, không tham số) → `StudentQueryHandler.HandleAsync(GetStudentsQuery)` → `IUnitOfWork.Students.GetAllAsync()`.
- Đọc theo id: `GET /api/students/{id}` dùng `GetStudentByIdQuery(Id)` → `StudentQueryHandler.HandleAsync(GetStudentByIdQuery)` → `IUnitOfWork.Students.GetByIdAsync(id)`.
- Nếu không tìm thấy record, handler trả về `null` và controller trả `404 NotFound` thay vì throw exception — giữ Application layer không phụ thuộc HTTP status code.

### 6.3 Update
- API: `PUT /api/students/{id}`, nhận body là `UpdateStudentCommand(Id, FullName, Age, Email)`.
- Controller kiểm tra `id` trên route phải khớp `command.Id` trong body, tránh cập nhật nhầm resource.
- Handler: `StudentCommandHandler.UpdateAsync` load entity hiện có bằng `GetByIdAsync`, gán lại field, gọi `UpdateAsync` + `SaveChangesAsync`. Trả `null` nếu không tìm thấy để controller trả `404`.
- Đây là pattern "load - mutate - save", phù hợp với EF Core change tracking thay vì update trực tiếp bằng SQL.

### 6.4 Delete
- API: `DELETE /api/students/{id}` dùng `DeleteStudentCommand(Id)`.
- Handler: `StudentCommandHandler.DeleteAsync` kiểm tra record tồn tại trước khi xoá, trả `bool` cho biết có xoá được không.
- Controller trả `204 NoContent` khi xoá thành công, `404 NotFound` khi không tìm thấy.

### 6.5 Nguyên tắc chung khi thêm CRUD cho entity mới
1. Định nghĩa đủ 5 method trong repository interface ở Domain: `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`.
2. Ở Application, tạo command cho Create/Update/Delete và query cho GetAll/GetById — mỗi command/query là một record riêng, không gộp chung.
3. Handler không throw exception cho trường hợp "not found" ở mức nghiệp vụ bình thường — trả `null`/`bool` để controller tự quyết định status code.
4. Controller chỉ làm nhiệm vụ điều phối HTTP: nhận request, gọi handler, map kết quả sang status code phù hợp (`200`, `201`, `204`, `404`, `400`).
5. Luôn dùng `CancellationToken` xuyên suốt các layer để hỗ trợ hủy request khi client disconnect.
