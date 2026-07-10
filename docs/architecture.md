# Architecture Guide

## 1. Tổng quan
Dự án này là một mẫu .NET 8 theo kiến trúc Onion Architecture, dùng để minh họa cách chia code thành các lớp có trách nhiệm rõ ràng. Mục tiêu là giữ cho business logic không bị lẫn với truy vấn database, DI, hay HTTP controller.

## 2. Cấu trúc các layer

### Layer 1: Domain
Đây là lớp trung tâm, chứa các phần cốt lõi của business domain.

Responsibilities:
- Định nghĩa entity như Student.
- Định nghĩa interface repository như IStudentRepository và IUnitOfWork.
- Chứa các rule nghiệp vụ quan trọng nếu có.

Ví dụ trong project:
- [BaseBE.Domain/Entities/Student.cs](../BaseBE.Domain/Entities/Student.cs)
- [BaseBE.Domain/Repositories/IStudentRepository.cs](../BaseBE.Domain/Repositories/IStudentRepository.cs)
- [BaseBE.Domain/Repositories/IUnitOfWork.cs](../BaseBE.Domain/Repositories/IUnitOfWork.cs)

Lưu ý quan trọng:
- Domain không nên biết về EF Core, SQL Server, ASP.NET Core hay controller.
- Domain chỉ nói về “điều gì là đúng trong business” chứ không nói “làm thế nào lưu dữ liệu”.

### Layer 2: Application
Lớp này chứa các use case của hệ thống: tạo học sinh, lấy danh sách học sinh, xử lý request trước khi đưa xuống data layer.

Responsibilities:
- Chứa command, query, DTO và handler.
- Tập trung orchestration logic giữa request và domain.
- Không trực tiếp xử lý HTTP, chỉ xử lý nghiệp vụ ứng dụng.

Ví dụ trong project:
- [BaseBE.Application/Commands/CreateStudentCommand.cs](../BaseBE.Application/Commands/CreateStudentCommand.cs)
- [BaseBE.Application/Commands/UpdateStudentCommand.cs](../BaseBE.Application/Commands/UpdateStudentCommand.cs)
- [BaseBE.Application/Commands/DeleteStudentCommand.cs](../BaseBE.Application/Commands/DeleteStudentCommand.cs)
- [BaseBE.Application/Queries/GetStudentsQuery.cs](../BaseBE.Application/Queries/GetStudentsQuery.cs)
- [BaseBE.Application/Queries/GetStudentByIdQuery.cs](../BaseBE.Application/Queries/GetStudentByIdQuery.cs)
- [BaseBE.Application/DTOs/StudentDto.cs](../BaseBE.Application/DTOs/StudentDto.cs)
- [BaseBE.Application/Handlers/StudentCommandHandler.cs](../BaseBE.Application/Handlers/StudentCommandHandler.cs)
- [BaseBE.Application/Handlers/StudentQueryHandler.cs](../BaseBE.Application/Handlers/StudentQueryHandler.cs)

### Layer 3: Infrastructure
Lớp này triển khai chi tiết kỹ thuật cho các interface đã được định nghĩa ở Domain và Application.

Responsibilities:
- Triển khai repository.
- Cấu hình DbContext với EF Core.
- Tạo object, mapping, kết nối database.

Ví dụ trong project:
- [BaseBE.Infrastructure/Data/ApplicationDbContext.cs](../BaseBE.Infrastructure/Data/ApplicationDbContext.cs)
- [BaseBE.Infrastructure/Repositories/StudentRepository.cs](../BaseBE.Infrastructure/Repositories/StudentRepository.cs)
- [BaseBE.Infrastructure/Repositories/UnitOfWork.cs](../BaseBE.Infrastructure/Repositories/UnitOfWork.cs)
- [BaseBE.Infrastructure/Factories/StudentFactory.cs](../BaseBE.Infrastructure/Factories/StudentFactory.cs)

### Layer 4: API
Lớp này là entry point cho client. Nó nhận HTTP request, gọi Application layer rồi trả response.

Responsibilities:
- Định nghĩa endpoint.
- Parse request body/query.
- Trả về status code và payload phù hợp.

Ví dụ trong project:
- [BaseBE.API/Controllers/StudentsController.cs](../BaseBE.API/Controllers/StudentsController.cs)
- [BaseBE.API/Program.cs](../BaseBE.API/Program.cs)

## 3. Luồng xử lý thực tế
### Scenario: tạo học sinh mới
1. Client gửi request POST đến /api/students.
2. StudentsController nhận CreateStudentCommand từ body.
3. StudentCommandHandler tạo entity Student mới.
4. Handler gọi IUnitOfWork để lưu dữ liệu.
5. Repository thực hiện insert vào database qua EF Core.
6. Controller trả về response 201 Created cùng dữ liệu vừa tạo.

### Scenario: lấy danh sách học sinh
1. Client gửi request GET đến /api/students.
2. Controller gọi StudentQueryHandler.
3. Query handler gọi IUnitOfWork.Students.GetAllAsync().
4. Repository đọc dữ liệu từ database.
5. Controller trả về danh sách dạng JSON.

### Scenario: lấy chi tiết một học sinh
1. Client gửi request GET đến /api/students/{id}.
2. Controller tạo GetStudentByIdQuery(id) rồi gọi StudentQueryHandler.
3. Query handler gọi IUnitOfWork.Students.GetByIdAsync(id).
4. Nếu không tìm thấy, handler trả về null và controller trả 404 NotFound.
5. Nếu tìm thấy, controller trả 200 OK cùng StudentDto.

### Scenario: cập nhật học sinh
1. Client gửi request PUT đến /api/students/{id} kèm UpdateStudentCommand.
2. Controller kiểm tra id trên route khớp với Id trong body, nếu không trả 400 BadRequest.
3. StudentCommandHandler.UpdateAsync load entity hiện có, gán lại field mới.
4. Handler gọi IUnitOfWork.Students.UpdateAsync rồi SaveChangesAsync.
5. Controller trả 200 OK cùng StudentDto đã cập nhật, hoặc 404 NotFound nếu không tìm thấy entity.

### Scenario: xoá học sinh
1. Client gửi request DELETE đến /api/students/{id}.
2. Controller tạo DeleteStudentCommand(id) rồi gọi StudentCommandHandler.DeleteAsync.
3. Handler kiểm tra entity tồn tại, nếu có thì gọi IUnitOfWork.Students.DeleteAsync rồi SaveChangesAsync.
4. Controller trả 204 NoContent khi xoá thành công, hoặc 404 NotFound nếu không tìm thấy entity.

## 4. Hướng dependency
Quan hệ phụ thuộc nên đi theo chiều từ ngoài vào trong:
- API -> Application -> Domain
- Infrastructure -> Domain

Nguyên tắc quan trọng:
- Domain không phụ thuộc vào Infrastructure.
- Application chỉ phụ thuộc vào abstractions ở Domain.
- Infrastructure mới là nơi implement các abstractions đó.

## 5. Lợi ích của kiến trúc này
- Dễ bảo trì vì mỗi layer có trách nhiệm riêng.
- Dễ test vì có thể mock repository hoặc handler.
- Dễ thay đổi database hoặc framework vì chỉ ảnh hưởng tới Infrastructure.
- Dễ mở rộng feature mới mà không làm spaghetti code.

## 6. Khi nào nên dùng kiến trúc này?
- Khi dự án có nhiều module và logic nghiệp vụ rõ ràng.
- Khi cần tách riêng business logic khỏi infrastructure.
- Khi bạn muốn làm mẫu học tập hoặc dự án production scale trung bình.

## 7. Gợi ý cách đọc source code
Để hiểu nhanh nhất, hãy đọc theo thứ tự sau:
1. Entity và repository interface ở Domain.
2. Command/query/handler ở Application.
3. DbContext và repository ở Infrastructure.
4. Controller và DI registration ở API.

Điều này giúp bạn hiểu “flow” của một request từ đầu đến cuối.
