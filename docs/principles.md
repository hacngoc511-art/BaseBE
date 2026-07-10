# Principles Guide

Tài liệu này giải thích các nguyên tắc thiết kế phần mềm (OOP, SOLID, KISS, DRY, YAGNI) và chỉ ra ví dụ thực tế từ source code trong project để minh hoạ, thay vì chỉ nói lý thuyết suông.

## 1. OOP — 4 tính chất cơ bản

### 1.1 Encapsulation (Đóng gói)
Đóng gói nghĩa là giấu chi tiết cài đặt bên trong, chỉ lộ ra hành vi cần thiết.

Ví dụ trong project:
- `Student` ([BaseBE.Domain/Entities/Student.cs](../BaseBE.Domain/Entities/Student.cs)) tự khởi tạo `Id` bằng `Guid.NewGuid()` và `CreatedAt` bằng `DateTime.UtcNow` ngay tại property default, nên caller không cần (và không nên) tự gán các giá trị này.
- `StudentRepository` giấu hoàn toàn cách EF Core truy vấn (`FirstOrDefaultAsync`, `OrderBy`...) phía sau interface `IStudentRepository`. `StudentCommandHandler` chỉ gọi `_unitOfWork.Students.AddAsync(...)` mà không biết bên trong dùng SQL Server hay provider nào khác.

Ví dụ dễ hiểu hơn (ngoài project):
- Một chiếc **máy giặt**: bạn chỉ cần bấm nút "Start", không cần biết bên trong nó cấp nước, quay lồng giặt, xả nước theo trình tự nào. Nhà sản xuất "giấu" toàn bộ mạch điện, motor bên trong vỏ máy, chỉ lộ ra bảng điều khiển.
- Trong code, một class `BankAccount` với field `_balance` là `private`, chỉ cho phép thay đổi qua method `Deposit(amount)`/`Withdraw(amount)` có kiểm tra hợp lệ (không cho rút quá số dư). Nếu `_balance` là `public`, ai cũng có thể gán trực tiếp `account.Balance = -1000000`, phá vỡ tính toàn vẹn dữ liệu — đó là lý do cần đóng gói.

### 1.2 Abstraction (Trừu tượng hoá)
Abstraction nghĩa là định nghĩa "cái gì cần làm" tách khỏi "làm như thế nào".

Ví dụ trong project:
- `IStudentRepository` và `IUnitOfWork` ([BaseBE.Domain/Repositories/](../BaseBE.Domain/Repositories/)) chỉ khai báo hành vi (`GetByIdAsync`, `AddAsync`, `SaveChangesAsync`...), không có bất kỳ dòng code EF Core nào. Toàn bộ Application layer (`StudentCommandHandler`, `StudentQueryHandler`) chỉ phụ thuộc vào các abstraction này, không phụ thuộc `StudentRepository`/`UnitOfWork` cụ thể.

Ví dụ dễ hiểu hơn (ngoài project):
- **Vô lăng ô tô**: mọi hãng xe đều dùng vô lăng để rẽ trái/phải, dù cơ chế lái bên trong (trợ lực dầu, trợ lực điện...) khác nhau hoàn toàn. Người lái chỉ cần biết "xoay vô lăng" (abstraction), không cần biết cơ chế thật bên dưới.
- Interface `IPaymentGateway` với method `Pay(amount)` trong một hệ thống thương mại điện tử: code đặt hàng chỉ gọi `paymentGateway.Pay(100000)`, không quan tâm đó là Momo, VNPay hay Stripe — miễn là implementation nào cũng tuân theo cùng một "hợp đồng" `Pay(amount)`.

### 1.3 Inheritance (Kế thừa)
Kế thừa dùng để chia sẻ hành vi chung giữa các class liên quan.

Ví dụ trong project:
- `ApplicationDbContext : DbContext` ([BaseBE.Infrastructure/Data/ApplicationDbContext.cs](../BaseBE.Infrastructure/Data/ApplicationDbContext.cs)) kế thừa `DbContext` của EF Core để có sẵn hành vi change tracking, `SaveChangesAsync`, LINQ provider... và chỉ override `OnModelCreating` để khai báo mapping riêng cho `Student`.
- `StudentsController : ControllerBase` kế thừa `ControllerBase` để có sẵn `Ok()`, `NotFound()`, `CreatedAtAction()`... thay vì phải tự viết logic tạo `IActionResult`.

Lưu ý: project hiện tại **ưu tiên composition hơn inheritance** ở tầng nghiệp vụ — `Student` không có class cha nghiệp vụ nào, `StudentCommandHandler` không kế thừa base handler. Đây là lựa chọn hợp lý vì kế thừa sâu giữa các entity nghiệp vụ dễ tạo ra phân cấp cứng nhắc, khó mở rộng hơn so với dùng interface + composition (giống cách `UnitOfWork` "có" các repository thay vì "là" một loại repository).

Ví dụ dễ hiểu hơn (ngoài project):
- Sinh vật học: `Dog`, `Cat`, `Bird` đều kế thừa từ `Animal` để dùng chung thuộc tính như `Name`, `Age` và method `Eat()`, mỗi loài chỉ cần override phần riêng của mình (`Bird` override `Move()` thành bay, `Dog`/`Cat` giữ nguyên đi bằng chân).
- Trong UI, `Button`, `Checkbox`, `TextBox` đều kế thừa từ `Control` để có sẵn `Width`, `Height`, `Visible`, tránh phải viết lại các thuộc tính chung đó ở từng loại control.

### 1.4 Polymorphism (Đa hình)
Đa hình nghĩa là nhiều class khác nhau có thể được gọi qua cùng một interface, hành vi thực thi tuỳ theo class cụ thể.

Ví dụ trong project:
- Bất kỳ chỗ nào nhận `IStudentRepository` (như constructor của `UnitOfWork`) đều có thể nhận `StudentRepository` thật hoặc một implementation khác (ví dụ fake repository dùng cho unit test) mà code gọi không cần đổi gì. Đây chính là đa hình thông qua interface, được ASP.NET Core DI container hiện thực hoá lúc runtime khi `Program.cs` đăng ký `AddScoped<IUnitOfWork, UnitOfWork>()`.

Ví dụ dễ hiểu hơn (ngoài project):
- Gọi `animal.MakeSound()` trên một danh sách `List<Animal>` chứa cả `Dog` và `Cat`: con `Dog` sẽ "Sủa", con `Cat` sẽ "Meo meo" — cùng một lời gọi method, nhưng hành vi thực thi khác nhau tuỳ đối tượng cụ thể tại runtime.
- Trong đời thường: cùng thao tác "quẹt thẻ thanh toán" tại máy POS, nhưng máy sẽ xử lý khác nhau tuỳ loại thẻ (thẻ tín dụng trừ qua ngân hàng, thẻ trả trước trừ qua ví nội bộ) — người dùng chỉ cần "quẹt thẻ", không cần biết loại thẻ nào sẽ được xử lý ra sao.

## 2. SOLID

### 2.1 S — Single Responsibility Principle (Nguyên tắc đơn nhiệm)
Một class chỉ nên có một lý do để thay đổi.

Ví dụ trong project:
- `StudentCommandHandler` chỉ lo việc ghi dữ liệu (Create/Update/Delete), `StudentQueryHandler` chỉ lo việc đọc dữ liệu (GetAll/GetById) — đây chính là biểu hiện của CQRS kết hợp SRP: nếu logic đọc thay đổi (ví dụ thêm cache), chỉ `StudentQueryHandler` cần sửa, không ảnh hưởng logic ghi.
- `StudentsController` chỉ lo nhận HTTP request và map kết quả sang status code, không tự query database hay chứa business rule — lý do duy nhất khiến controller thay đổi là khi contract HTTP (route, status code) thay đổi.

Ví dụ dễ hiểu hơn (ngoài project):
- Một class `Invoice` chỉ nên chứa logic tính tổng tiền hoá đơn, không nên kiêm luôn việc `PrintInvoice()` (in ấn) và `SaveToDatabase()` (lưu trữ). Nếu gộp chung, khi định dạng in thay đổi hoặc đổi database, class `Invoice` đều phải sửa — vi phạm SRP. Nên tách thành `Invoice` (tính toán), `InvoicePrinter` (in), `InvoiceRepository` (lưu).
- Một đầu bếp chỉ nấu ăn, không kiêm luôn thu ngân và bảo vệ — mỗi vai trò trong nhà hàng chỉ có "một lý do để thay đổi cách làm việc" (đầu bếp đổi vì công thức món ăn đổi, thu ngân đổi vì chính sách giá đổi).

### 2.2 O — Open/Closed Principle (Nguyên tắc đóng/mở)
Class nên mở để mở rộng nhưng đóng để sửa đổi.

Ví dụ trong project:
- `IStudentRepository` cho phép thêm một implementation hoàn toàn mới (ví dụ `MongoStudentRepository`) mà không cần sửa `StudentCommandHandler`/`StudentQueryHandler` — chỉ cần đổi dòng đăng ký DI trong `Program.cs`. Đây là cách kiến trúc "mở" cho việc mở rộng storage nhưng "đóng" với việc sửa Application layer.

Ví dụ dễ hiểu hơn (ngoài project):
- Một hệ thống tính phí ship: thay vì viết `if (type == "standard") ... else if (type == "express") ...` rồi mỗi lần thêm loại ship mới lại phải sửa hàm đó, nên định nghĩa interface `IShippingStrategy` với method `CalculateFee(order)`, mỗi loại ship (`StandardShipping`, `ExpressShipping`, `SameDayShipping`) là một class implement riêng. Thêm loại ship mới = thêm class mới, không sửa code cũ.
- Ổ cắm điện trong nhà: bạn có thể cắm thêm bất kỳ thiết bị mới nào (quạt, sạc điện thoại, nồi cơm) vào ổ cắm có sẵn mà không cần "sửa lại" hệ thống dây điện trong tường — hệ thống điện "mở" để cắm thêm thiết bị nhưng "đóng" với việc phải đục tường sửa dây mỗi lần có thiết bị mới.

### 2.3 L — Liskov Substitution Principle (Nguyên tắc thay thế Liskov)
Bất kỳ implementation nào của một interface đều phải thay thế được cho nhau mà không phá vỡ hành vi mong đợi của caller.

Ví dụ trong project:
- Mọi implementation của `IStudentRepository` đều phải giữ đúng hợp đồng: `GetByIdAsync` trả `null` khi không tìm thấy (không throw exception), `GetAllAsync` luôn trả `List<Student>` (không trả `null`). `StudentQueryHandler` code dựa trên hợp đồng này (`student is null ? null : new StudentDto(...)`) — nếu một implementation khác throw exception thay vì trả `null` khi không tìm thấy, nó vi phạm LSP và sẽ làm sai luồng 404 của controller.

Ví dụ dễ hiểu hơn (ngoài project):
- Ví dụ kinh điển: class `Square` (hình vuông) kế thừa từ `Rectangle` (hình chữ nhật) và override `SetWidth`/`SetHeight` để luôn giữ width = height. Code caller mong đợi `rectangle.SetWidth(5); rectangle.SetHeight(10);` sẽ cho diện tích 50, nhưng với `Square` lại ra 100 — `Square` "trông giống" `Rectangle` nhưng phá vỡ hành vi mong đợi, vi phạm LSP dù về mặt hình học `Square` đúng là một loại `Rectangle`.
- Nếu một app có `IEmailSender.Send(to, subject, body)` mà implementation `FakeEmailSender` dùng để test lại throw `NotImplementedException`, thì bất kỳ code nào gọi `Send()` qua interface này với `FakeEmailSender` sẽ crash bất ngờ — vi phạm kỳ vọng "mọi implementation của interface đều dùng thay thế được cho nhau".

### 2.4 I — Interface Segregation Principle (Nguyên tắc phân tách interface)
Không nên ép class implement những method nó không dùng tới.

Ví dụ trong project:
- `IStudentRepository` chỉ chứa đúng 5 method cần cho Student (`GetByIdAsync`, `GetAllAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`), không gộp chung vào một interface khổng lồ kiểu `IGenericRepository` chứa hàng chục method cho mọi entity. Khi thêm entity mới (ví dụ `Course`), nên tạo `ICourseRepository` riêng thay vì nhồi thêm method `Course...` vào `IStudentRepository` — giữ mỗi interface gọn và đúng phạm vi.

Ví dụ dễ hiểu hơn (ngoài project):
- Interface `IMultiFunctionPrinter` với `Print()`, `Scan()`, `Fax()`: nếu một máy in bình dân chỉ in được, nó vẫn bị buộc phải implement `Scan()`/`Fax()` (thường là throw `NotSupportedException`). Nên tách thành `IPrinter`, `IScanner`, `IFax` riêng — máy nào làm được gì thì chỉ implement interface đó.
- Ở quán ăn, thực đơn "combo tất cả trong một" bắt khách phải gọi cả súp, salad, tráng miệng dù chỉ muốn ăn món chính — tốt hơn là để khách chọn từng món riêng lẻ (interface nhỏ, khách "implement" đúng cái mình cần).

### 2.5 D — Dependency Inversion Principle (Nguyên tắc đảo ngược phụ thuộc)
Module cấp cao không nên phụ thuộc module cấp thấp, cả hai nên phụ thuộc vào abstraction.

Ví dụ trong project — đây là nguyên tắc nền tảng của toàn bộ Onion Architecture trong repo:
- `StudentCommandHandler` (Application, cấp cao) không phụ thuộc trực tiếp `StudentRepository`/`ApplicationDbContext` (Infrastructure, cấp thấp). Cả hai cùng phụ thuộc vào `IUnitOfWork`/`IStudentRepository` được định nghĩa ở Domain.
- Việc "ai implement interface nào" chỉ được quyết định một chỗ duy nhất: `Program.cs` (`builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();`). Đây là composition root — nơi duy nhất đảo chiều phụ thuộc từ abstraction sang implementation cụ thể.

Ví dụ dễ hiểu hơn (ngoài project):
- Ổ cắm điện chuẩn quốc gia là một "abstraction": nhà sản xuất bóng đèn, quạt, tivi đều thiết kế phích cắm theo chuẩn đó, không ai chế tạo thiết bị phụ thuộc trực tiếp vào một hãng dây điện cụ thể. Nhà và thiết bị điện đều phụ thuộc vào "chuẩn ổ cắm" chứ không phụ thuộc lẫn nhau.
- Trong code, class `OrderService` (cấp cao) không nên `new SqlOrderRepository()` trực tiếp bên trong nó — nếu làm vậy, muốn đổi sang lưu file hay MongoDB phải sửa `OrderService`. Thay vào đó, `OrderService` nhận `IOrderRepository` qua constructor, còn việc "dùng repository nào" được quyết định ở nơi khởi tạo ứng dụng (composition root) — giống hệt cách `Program.cs` quyết định `UnitOfWork` trong project này.

## 3. KISS — Keep It Simple, Stupid
Ưu tiên giải pháp đơn giản, dễ hiểu hơn là giải pháp "thông minh" nhưng khó đọc.

Ví dụ trong project:
- CQRS ở đây được triển khai ở dạng đơn giản nhất: controller gọi thẳng handler qua constructor injection, không có mediator, không có pipeline behavior phức tạp. Với quy mô một feature Student, đây là mức đủ dùng — không cần thêm MediatR nếu chưa thật sự cần pipeline (logging, validation) chạy xuyên suốt nhiều handler.
- `StudentFactory.Create(...)` chỉ đơn giản là `new Student { ... }` — chưa có rule phức tạp thì không cần thêm logic phức tạp vào factory. Factory tồn tại để có "chỗ" mở rộng sau này, không phải để tỏ ra phức tạp ngay từ đầu.

Ví dụ dễ hiểu hơn (ngoài project):
- Cần kiểm tra một số có phải số chẵn không: `return number % 2 == 0;` là đủ. Không cần viết hẳn một `EvenNumberStrategyFactory` với `IEvenCheckStrategy` chỉ để làm một phép toán một dòng.
- Điều khiển TV: nút "Power" chỉ cần một nút bấm để bật/tắt. Nếu thiết kế lại thành một quy trình 5 bước (xác nhận, chọn chế độ, xác nhận lại...) chỉ để bật TV, đó là làm phức tạp hoá một việc vốn đơn giản — vi phạm KISS.

## 4. DRY — Don't Repeat Yourself
Tránh lặp lại cùng một logic ở nhiều nơi.

Ví dụ trong project:
- Cả `StudentCommandHandler` và `StudentQueryHandler` đều map `Student` → `StudentDto` bằng cách viết `new StudentDto(s.Id, s.FullName, s.Age, s.Email, s.CreatedAt)` lặp lại ở nhiều chỗ (`CreateAsync`, `UpdateAsync`, `HandleAsync(GetStudentsQuery)`, `HandleAsync(GetStudentByIdQuery)`). Đây là một vi phạm DRY nhỏ đang tồn tại trong code hiện tại — điểm này được ghi chú thêm ở [recommend.md](recommend.md) mục 5 (AutoMapper hoặc mapping tay có tổ chức) như một hướng cải thiện.

Ví dụ dễ hiểu hơn (ngoài project):
- Nếu công thức tính "giá sau thuế" (`price * 1.1`) được copy-paste ở 5 chỗ khác nhau trong code (trang giỏ hàng, trang thanh toán, báo cáo, email xác nhận, API export), khi thuế suất đổi từ 10% sang 8%, phải nhớ sửa đúng cả 5 chỗ — rất dễ sót. Nên viết một hàm `CalculatePriceAfterTax(price)` dùng chung.
- Trong đời thường: thay vì mỗi phòng ban trong công ty tự in một bản "quy định nghỉ phép" riêng (dễ lệch nhau theo thời gian), công ty nên có một tài liệu quy định chung, mọi phòng ban đều tham chiếu tới cùng một nguồn.

## 5. YAGNI — You Aren't Gonna Need It
Không xây dựng tính năng/abstraction cho nhu cầu chưa xảy ra.

Ví dụ trong project:
- Repository hiện tại được viết riêng cho từng entity (`IStudentRepository`) thay vì làm `IRepository<T>` generic ngay từ đầu, vì project mới chỉ có một entity Student. Generic repository sẽ hợp lý hơn khi có nhiều entity dùng chung pattern CRUD giống hệt nhau — làm sớm quá khi chưa cần sẽ chỉ tăng độ trừu tượng không cần thiết.
- `IUnitOfWork` hiện chỉ có một property `Students` — không tự thêm sẵn các property cho entity chưa tồn tại (`Courses`, `Enrollments`...). Chỉ nên thêm khi entity đó thật sự được implement.

Ví dụ dễ hiểu hơn (ngoài project):
- Một startup mới có 2 người dùng thử nghiệm mà đã đầu tư xây hệ thống hạ tầng chịu tải 1 triệu người dùng đồng thời (auto-scaling, sharding database, CDN toàn cầu) — tốn công sức và tiền bạc cho nhu cầu chưa xảy ra, trong khi lẽ ra nên tập trung xác thực sản phẩm trước.
- Một quán cà phê nhỏ mới mở mà đã mua sẵn 10 máy pha cà phê công nghiệp "phòng khi đông khách" thay vì mua 1-2 máy đủ dùng rồi mua thêm khi thực sự cần — vốn bị chôn vào thứ chưa cần dùng tới.

## 6. Separation of Concerns (Tách biệt mối quan tâm)
Đây là nguyên tắc bao trùm toàn bộ kiến trúc 4 layer của project (xem chi tiết ở [architecture.md](architecture.md)):
- Domain: quan tâm "nghiệp vụ là gì".
- Application: quan tâm "use case xử lý ra sao".
- Infrastructure: quan tâm "lưu trữ/kỹ thuật thế nào".
- API: quan tâm "giao tiếp HTTP ra sao".

Ví dụ dễ hiểu hơn (ngoài project):
- Một nhà hàng tách biệt rõ ràng: đầu bếp (nấu ăn) không đứng thu ngân, thu ngân (tính tiền) không đi bưng bê, phục vụ (giao tiếp khách) không vào bếp nấu. Mỗi bộ phận chỉ "quan tâm" đúng phần việc của mình, nếu công thức món ăn đổi thì chỉ cần huấn luyện lại đầu bếp, không ảnh hưởng tới thu ngân.
- Trong một chiếc xe hơi: động cơ (sinh lực kéo), hệ thống phanh (dừng xe), và bảng đồng hồ (hiển thị thông tin) là 3 hệ thống tách biệt — thợ sửa hệ thống phanh không cần hiểu logic hiển thị đồng hồ tốc độ.

Mỗi khi thêm code mới, nên tự hỏi: đoạn code này thuộc mối quan tâm nào trong 4 loại trên, rồi đặt đúng layer tương ứng — đây là cách thực hành cụ thể nhất của Separation of Concerns trong project này.

## 7. Cách áp dụng các nguyên tắc này khi review code
Khi review một pull request trong project, có thể tự hỏi:
1. (SRP) Class/method này có đang làm nhiều hơn một việc không?
2. (DIP) Có đang phụ thuộc trực tiếp vào implementation (EF Core, SQL) ở Application/Domain layer không?
3. (KISS/YAGNI) Có đang thêm abstraction cho một nhu cầu chưa thực sự tồn tại không?
4. (DRY) Logic mapping/validate có đang bị lặp lại ở nhiều handler không?
5. (LSP) Nếu thêm implementation mới cho một interface, nó có giữ đúng hợp đồng (return null vs throw, v.v.) như implementation cũ không?

Trả lời được 5 câu hỏi này giúp giữ code trong project luôn nhất quán với kiến trúc và pattern đã chọn.
