# Hướng dẫn sử dụng source

## 1. Cách đọc project hiệu quả
Đây là một project mẫu nên cách học tốt nhất là đọc từ trong ra ngoài:
1. Bắt đầu từ Domain để hiểu domain model.
2. Đọc Application để thấy use case và cách handler phối hợp logic.
3. Xem Infrastructure để hiểu cách dữ liệu được lưu và truy xuất.
4. Cuối cùng xem API để thấy request/response đi qua đâu.

## 2. Các thành phần chính bạn nên biết
### Domain
- Chứa entity và interface abstraction.
- Đây là nơi “ngôn ngữ nghiệp vụ” được định nghĩa.
- Ví dụ: Student có FullName, Age, Email.

### Application
- Chứa command/query và handler.
- Handler là nơi quyết định logic ứng dụng, như “tạo student mới” hoặc “lấy tất cả student”.

### Infrastructure
- Là nơi triển khai repository, DbContext, factory.
- Mọi thứ liên quan EF Core, SQL Server, migration đều nằm ở đây.

### API
- Lớp ngoài cùng, nhận HTTP request.
- Controller không nên chứa logic nghiệp vụ quá nhiều; nên chỉ chuyển request vào handler.

## 3. Cách chạy dự án
### Bước 1: Kiểm tra môi trường
```bash
dotnet --version
```

### Bước 2: Restore dependencies
```bash
dotnet restore
```

### Bước 3: Cấu hình connection string
File cấu hình nằm tại:
- [BaseBE.API/appsettings.json](../BaseBE.API/appsettings.json)
- [BaseBE.API/appsettings.Development.json](../BaseBE.API/appsettings.Development.json)

Bạn nên đảm bảo có connection string tên DefaultConnection.

### Bước 4: Tạo database (nếu lần đầu chạy)
```bash
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate --project BaseBE.Infrastructure --startup-project BaseBE.API
dotnet ef database update --project BaseBE.Infrastructure --startup-project BaseBE.API
```

### Bước 5: Chạy ứng dụng
```bash
dotnet run --project BaseBE.API
```

Sau khi chạy, bạn có thể mở:
- Swagger UI: https://localhost:5001/swagger
- Endpoint mẫu: https://localhost:5001/api/students

## 4. Cách test API bằng Swagger hoặc Postman
Feature Student cung cấp đầy đủ CRUD (Create, Read, Update, Delete).

### GET /api/students
Lấy danh sách học sinh. Trả về `200 OK` cùng mảng `StudentDto`.

### GET /api/students/{id}
Lấy chi tiết một học sinh theo id. Trả về `200 OK` nếu tìm thấy, `404 NotFound` nếu không.

### POST /api/students
Tạo học sinh mới. Body mẫu:
```json
{
  "fullName": "Nguyen Van A",
  "age": 16,
  "email": "a@example.com"
}
```

Nếu thành công, server sẽ trả về status 201 Created, header Location trỏ tới GET /api/students/{id}, và dữ liệu học sinh vừa tạo.

### PUT /api/students/{id}
Cập nhật học sinh đã tồn tại. Body mẫu:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "Nguyen Van A",
  "age": 17,
  "email": "a@example.com"
}
```
`id` trong body phải khớp `{id}` trên route, nếu không sẽ nhận `400 BadRequest`. Thành công trả về `200 OK` cùng dữ liệu đã cập nhật, `404 NotFound` nếu không tìm thấy học sinh.

### DELETE /api/students/{id}
Xoá học sinh theo id. Thành công trả về `204 NoContent`, `404 NotFound` nếu không tìm thấy.

## 5. Cách thêm một feature mới
Ví dụ bạn muốn thêm module Course hoặc Category.

### Bước 1: Tạo entity trong Domain
- Tạo class mới trong thư mục Entities.
- Định nghĩa thuộc tính và các rule bắt buộc.

### Bước 2: Tạo interface repository
- Thêm interface trong thư mục Repositories của Domain.
- Interface nên mô tả hành vi cần thiết, ví dụ: AddAsync, GetAllAsync, GetByIdAsync.

### Bước 3: Tạo command/query và DTO trong Application
- Nếu feature thay đổi dữ liệu: thêm command.
- Nếu feature đọc dữ liệu: thêm query.
- DTO dùng để định dạng dữ liệu trả về cho client.

### Bước 4: Tạo handler
- Handler là nơi thực hiện logic use case.
- Ví dụ: CourseCommandHandler tạo course mới.

### Bước 5: Implement repository trong Infrastructure
- Tạo class repository implement interface ở Domain.
- Trong repository, dùng DbContext để thao tác database.

### Bước 6: Tạo controller và đăng ký DI
- Thêm controller trong API.
- Trong Program.cs, đăng ký handler và repository liên quan.

## 6. Những điều nên nhớ khi làm việc với project
- Đừng viết logic database trực tiếp trong controller.
- Đừng để controller xử lý quá nhiều logic nghiệp vụ.
- Đừng phụ thuộc trực tiếp vào EF Core trong Application layer.
- Nếu có thể, hãy giữ mỗi class chỉ làm một nhiệm vụ rõ ràng.

## 7. Mẹo học tập
- Hãy bắt đầu bằng feature Student vì đây là feature mẫu đã được triển khai đầy đủ.
- Khi đọc code, hãy trả lời 3 câu hỏi: “Layer này làm gì?”, “Ai gọi nó?”, “Ai phụ trách dữ liệu?”.
- Sau khi hiểu Student, hãy thử tự thêm một feature nhỏ như Category hoặc Course.
