# BaseBE - .NET Onion Architecture Sample

Đây là một base source .NET C# dùng để minh họa cách xây dựng một ứng dụng theo Onion Architecture với các pattern phổ biến: Repository, Unit of Work, Factory và CQRS.

## 1. Mục tiêu của project
Project này có mục đích học tập và làm nền tảng cho việc xây dựng ứng dụng thực tế với cấu trúc rõ ràng, dễ mở rộng và dễ bảo trì.

## 2. Kiến trúc tổng quan
Project được chia thành 4 layer chính:
- Domain: chứa entity, interface repository, các abstraction cốt lõi.
- Application: chứa use case, command/query, DTO, handler.
- Infrastructure: triển khai repository, DbContext, factory, kết nối database.
- API: entry point cho client, nhận HTTP request và trả response.

## 3. Cấu trúc thư mục chính
- [BaseBE.API](BaseBE.API): controllers, startup configuration, Swagger.
- [BaseBE.Application](BaseBE.Application): logic ứng dụng, handlers, command/query, DTO.
- [BaseBE.Domain](BaseBE.Domain): entity và contracts.
- [BaseBE.Infrastructure](BaseBE.Infrastructure): persistence và implement repository.
- [docs](docs): tài liệu hướng dẫn.

## 4. Các pattern được áp dụng
### Repository Pattern
- Ẩn chi tiết truy cập dữ liệu khỏi các layer trên.
- Interface nằm ở Domain, implementation nằm ở Infrastructure.

### Unit of Work
- Gộp nhiều thao tác persistence lại để lưu chung một lần.
- Giúp đảm bảo consistency khi có nhiều thay đổi liên quan.

### Factory Pattern
- Tạo object theo một quy trình chuẩn hóa.
- Ví dụ: StudentFactory.

### CQRS Pattern
- Phân tách thao tác đọc và ghi.
- Command handler dùng cho tạo dữ liệu, query handler dùng cho đọc dữ liệu.

## 5. Cách chạy dự án
### Bước 1: Cài đặt .NET SDK 8
```bash
dotnet --version
```

### Bước 2: Restore packages
```bash
dotnet restore
```

### Bước 3: Chạy migration lần đầu
```bash
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate --project BaseBE.Infrastructure --startup-project BaseBE.API
dotnet ef database update --project BaseBE.Infrastructure --startup-project BaseBE.API
```

### Bước 4: Chạy ứng dụng
```bash
dotnet run --project BaseBE.API
```

Sau đó mở browser:
- Swagger: https://localhost:5001/swagger
- API mẫu: https://localhost:5001/api/students

## 6. API mẫu
### GET /api/students
Lấy danh sách học sinh.

### POST /api/students
Tạo học sinh mới.

Body mẫu:
```json
{
  "fullName": "Nguyen Van A",
  "age": 16,
  "email": "a@example.com"
}
```

## 7. Vì sao nên dùng kiến trúc này?
- Dễ bảo trì và mở rộng.
- Logic nghiệp vụ tách biệt khỏi kỹ thuật triển khai.
- Dễ đổi database hoặc framework trong tương lai.
- Phù hợp cho việc học tập và làm nền tảng cho dự án thực tế.
