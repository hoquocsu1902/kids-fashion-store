# KẾ HOẠCH ĐỒ ÁN — Website quản lý mặt hàng thời trang trẻ em

> Tóm tắt baseline v1.0 (15/09/2026) từ kế hoạch đồ án gốc, dùng làm tài liệu tham chiếu chung của nhóm.
> Bản gốc PDF: [`Ke_hoach_do_an_Lap_trinh_Web.pdf`](Ke_hoach_do_an_Lap_trinh_Web.pdf).

---

## 1. Thông tin dự án và mục tiêu

| Hạng mục | Lựa chọn |
|---|---|
| Tên đề tài | Website quản lý mặt hàng thời trang trẻ em |
| Tên solution | KidsFashionStore |
| Tên database | QL_ThoiTrangTreEm |
| Mô hình | ASP.NET MVC 5 — .NET Framework |
| CSDL | Microsoft SQL Server |
| ORM chính | Entity Framework + LINQ (Database First) |
| Giao diện | Razor (.cshtml) + Bootstrap + CSS + JavaScript/jQuery |
| Quản lý trạng thái | Session cho User/Role/Cart |
| API | ASP.NET Web API — Product API cơ bản |
| Quản lý source | Git + GitHub |

**Mục tiêu định lượng:** ≥ 24 nhóm chức năng/tiêu chí rubric; 100% chức năng chính có testcase + bằng chứng;
không lỗi runtime ở luồng demo chính; database đủ PK/FK + dữ liệu mẫu; mỗi thành viên có module để vấn đáp;
bản chạy ổn định trước Tuần 14.

**Chiến lược điểm cao:** hoàn thành 100% tiêu chí trong bảng chấm trước khi thêm tính năng ngoài đề.
*Chức năng đúng + dữ liệu đúng + luồng ổn định + báo cáo có bằng chứng > nhiều tính năng không có trong rubric.*

## 2. Actor

| Actor | Quyền chính |
|---|---|
| Khách vãng lai | Xem sản phẩm, xem danh mục, tìm kiếm/lọc, xem chi tiết, đăng ký, đăng nhập |
| Khách hàng | Toàn bộ quyền khách vãng lai + giỏ hàng, đặt hàng, xem lịch sử đơn |
| Admin/Người bán | Đăng nhập quản trị, CRUD danh mục/sản phẩm, quản lý biến thể/hình, đơn hàng, khuyến mãi, doanh thu, API |

## 3. Module

| Mã | Module | Mô tả |
|---|---|---|
| M01 | Trang chủ & Layout | Header, menu, sản phẩm nổi bật/mới, footer, điều hướng |
| M02 | Danh mục & sản phẩm | Menu 2 cấp, danh sách, lọc danh mục, search, sort |
| M03 | Chi tiết sản phẩm | Thông tin, hình, biến thể size/màu, sản phẩm liên quan |
| M04 | Tài khoản | Đăng ký, đăng nhập, đăng xuất, Session User/Role |
| M05 | Giỏ hàng | Thêm, cập nhật số lượng, xóa dòng, xóa toàn bộ, tổng tiền |
| M06 | Đặt hàng | Xác nhận, lưu Orders + OrderDetails, lịch sử đơn |
| M07 | Admin danh mục/sản phẩm | CRUD Category/Product, hình, biến thể |
| M08 | Admin đơn hàng | Xem đơn, chi tiết, cập nhật trạng thái |
| M09 | Khuyến mãi & doanh thu | CRUD khuyến mãi, thống kê doanh thu |
| M10 | Web API | GET/POST/PUT/DELETE sản phẩm; test bằng browser/Postman |

## 4. Yêu cầu chức năng chi tiết (F01–F24)

| Mã | Chức năng | Yêu cầu | Nghiệm thu |
|---|---|---|---|
| F01 | Layout khách hàng | Layout dùng chung, menu điều hướng, footer, RenderBody | Tất cả trang khách kế thừa layout |
| F02 | Menu 2 cấp | Danh mục cha/con lấy động từ CSDL | Không hard-code danh mục |
| F03 | Danh sách sản phẩm | Card: ảnh, tên, giá, nút chi tiết | Lấy dữ liệu từ DB |
| F04 | Lọc theo danh mục | Nhấn danh mục → chỉ hiện sản phẩm thuộc danh mục | Đúng CategoryId |
| F05 | Tìm kiếm | Tìm theo tên gần đúng; kết hợp giá/danh mục | Không phân biệt rỗng/null |
| F06 | Sắp xếp | Tên A–Z hoặc giá tăng/giảm | Không làm mất điều kiện lọc |
| F07 | Chi tiết sản phẩm | Tên, giá, mô tả, ảnh chính/phụ, size, màu, tồn kho | Product không tồn tại → 404 |
| F08 | Sản phẩm liên quan | Cùng danh mục hoặc gần mức giá | Không lặp sản phẩm hiện tại |
| F09 | Đăng ký | Kiểm tra dữ liệu bắt buộc, trùng username/email | Tạo User role Customer |
| F10 | Đăng nhập/đăng xuất | Kiểm tra tài khoản; tạo/xóa Session | Hiển thị tên người dùng khi login |
| F11 | Thêm giỏ | Chọn variant + số lượng, thêm vào Session Cart | Trùng variant → cộng số lượng |
| F12 | Quản lý giỏ | Update/Delete/Clear; tính tổng số lượng/tổng tiền | Không cho số lượng ≤ 0 |
| F13 | Checkout | Yêu cầu login, nhập/xác nhận thông tin nhận hàng | Dữ liệu hợp lệ mới tạo đơn |
| F14 | Lưu đơn | Tạo Order và OrderDetail; kiểm tra tồn kho | Transaction logic nhất quán |
| F15 | Lịch sử đơn | Khách xem danh sách và chi tiết đơn của chính mình | Không xem đơn người khác |
| F16 | CRUD danh mục | Admin thêm/sửa/xóa/hiển thị category | Kiểm tra ràng buộc trước khi xóa |
| F17 | CRUD sản phẩm | Admin thêm/sửa/xóa/hiển thị product | Có category, price, image |
| F18 | Biến thể/hình ảnh | Quản lý size, màu, tồn kho và ảnh phụ | Không tạo variant trùng |
| F19 | Quản lý đơn | Admin xem và cập nhật trạng thái | Status theo tập giá trị chuẩn |
| F20 | Khuyến mãi | Thêm/sửa/xóa, thời gian hiệu lực, % giảm | StartDate ≤ EndDate |
| F21 | Doanh thu | Tổng đơn hoàn thành, tổng doanh thu, thống kê cơ bản | Dựa trên đơn Completed |
| F22 | Phân quyền | Trang admin yêu cầu Role=Admin | Customer bị redirect/deny |
| F23 | Web API | CRUD product qua HTTP methods | Response JSON/XML hợp lệ |
| F24 | Validation/UI | DataAnnotation/ViewModel, ValidationMessage, CSS/Bootstrap | Không submit dữ liệu sai phổ biến |

## 5. Kiến trúc và quy tắc

- **Luồng MVC:** Client Request → Routing chọn Controller/Action → Controller gọi EF/Model → SQL Server trả dữ liệu
  → Controller chọn View → Razor sinh HTML → Response. *(Cả nhóm phải hiểu để trình bày khi vấn đáp.)*
- Controller chỉ điều phối request/response; **không đặt HTML trong Controller**.
- View nhận dữ liệu từ Model/ViewModel; **không truy vấn SQL trực tiếp trong View**.
- Chuỗi kết nối đặt trong `Web.config`; không hard-code mật khẩu thật trong source public.
- Action thay đổi dữ liệu dùng `[HttpPost]` + `@Html.AntiForgeryToken()`.
- Chức năng admin kiểm tra Session/Role trước khi thực hiện.
- Không tạo 3 project riêng ghép cuối kỳ: cả nhóm làm trên **cùng một Solution + một schema DB**.

## 6. Thiết kế cơ sở dữ liệu

9 bảng: `Users`, `Categories`, `Products`, `ProductVariants`, `ProductImages`, `Orders`, `OrderDetails`,
`Promotions`, `ProductPromotions`. Có thể giản lược `ProductImages`/`ProductPromotions` nếu gấp,
nhưng **không bỏ** `Orders`/`OrderDetails`, `Categories`, `Products`, `Users`.

Chi tiết cột, quan hệ, quy ước và changelog: [`THIET_KE_DATABASE.md`](THIET_KE_DATABASE.md).
Script tạo DB + seed: `Database/QL_ThoiTrangTreEm.sql`.

**Quy ước dữ liệu:**

- ID dùng `INT IDENTITY`; FK đặt tên trùng khóa chính tham chiếu (`CategoryId`, `UserId`...).
- Giá tiền dùng `decimal`, không dùng `float`.
- `Status` đơn hàng: `Pending`, `Confirmed`, `Shipping`, `Completed`, `Cancelled`.
- `Role`: `Customer` hoặc `Admin`.
- Dữ liệu mẫu: ≥ 2 danh mục cha, 4–6 danh mục con, 15–20 sản phẩm, nhiều variant.

## 7. Cấu trúc project và quy ước

```
KidsFashionStore/
├── App_Start/       RouteConfig.cs, WebApiConfig.cs, FilterConfig.cs
├── Controllers/     Home, Product, Account, Cart, Order,
│                    AdminProduct, AdminCategory, AdminOrder,
│                    Promotion, Report, ProductApi
├── Models/          QL_ThoiTrangTreEm.edmx
│                    ViewModels/  LoginViewModel, RegisterViewModel,
│                                 ProductDetailViewModel, CheckoutViewModel
│                    Cart/CartItem.cs
├── Views/           Shared/ (_Layout, _AdminLayout, _CategoryMenu)
│                    Home, Product, Account, Cart, Order,
│                    AdminProduct, AdminCategory, AdminOrder, Promotion, Report
├── Content/         site.css, images/products/
├── Scripts/
├── Database/        QL_ThoiTrangTreEm.sql
├── Web.config
└── Global.asax
```

### 7.1. Quy tắc đặt tên

| Đối tượng | Quy tắc | Ví dụ |
|---|---|---|
| Controller | PascalCase + Controller | `ProductController`, `CartController` |
| Action | PascalCase | `Index`, `Detail`, `Search`, `AddToCart`, `Checkout` |
| Model/Entity | Danh từ số ít | `Product`, `Category`, `Order` |
| View | Trùng Action khi có thể | `Index.cshtml`, `Detail.cshtml` |
| Khóa chính | `<Entity>Id` | `ProductId`, `OrderId` |
| Khóa ngoại | Tên khóa tham chiếu | `CategoryId`, `UserId` |
| Branch | `feature/<owner>-<module>` | `feature/su-cart-order` |
| Commit | `type: mô tả ngắn` | `feat: add product search` |

## 8. Khung code chung (skeleton — điều chỉnh theo entity thật)

### 8.1. Controller đọc/lọc sản phẩm

```csharp
public class ProductController : Controller
{
    private readonly QL_ThoiTrangTreEmEntities db = new QL_ThoiTrangTreEmEntities();

    public ActionResult Index(int? categoryId, string keyword)
    {
        var query = db.Products.Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(p => p.ProductName.Contains(keyword));

        return View(query.ToList());
    }

    public ActionResult Detail(int id)
    {
        var product = db.Products.FirstOrDefault(p => p.ProductId == id);
        if (product == null) return HttpNotFound();
        return View(product);
    }
}
```

### 8.2. CartItem dùng Session

```csharp
public class CartItem
{
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    public string ProductName { get; set; }
    public string Image { get; set; }
    public string Size { get; set; }
    public string Color { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Price * Quantity;
}
```

### 8.3. Quy ước thao tác giỏ hàng

```csharp
private List<CartItem> GetCart()
{
    var cart = Session["Cart"] as List<CartItem>;
    if (cart == null)
    {
        cart = new List<CartItem>();
        Session["Cart"] = cart;
    }
    return cart;
}

public ActionResult AddToCart(int variantId, int quantity = 1)
{
    // 1. Lấy variant và product từ DB
    // 2. Kiểm tra tồn kho
    // 3. Nếu đã có trong cart -> tăng quantity
    // 4. Nếu chưa có -> thêm CartItem
    // 5. Redirect về Cart hoặc Detail
    return RedirectToAction("Index");
}
```

### 8.4. Khung Checkout

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Checkout(CheckoutViewModel model)
{
    if (Session["User"] == null)
        return RedirectToAction("Login", "Account");

    if (!ModelState.IsValid)
        return View(model);

    // Tạo Order
    // Tạo các OrderDetail từ Session["Cart"]
    // db.SaveChanges();
    // Session["Cart"] = null;
    return RedirectToAction("Success");
}
```

### 8.5. Kiểm tra quyền Admin

```csharp
private bool IsAdmin()
{
    return Session["Role"] != null && Session["Role"].ToString() == "Admin";
}

public ActionResult Index()
{
    if (!IsAdmin())
        return RedirectToAction("Login", "Account");

    return View(db.Products.ToList());
}
```

### 8.6. Khung Web API

```csharp
public class ProductApiController : ApiController
{
    private QL_ThoiTrangTreEmEntities db = new QL_ThoiTrangTreEmEntities();

    [HttpGet]
    public IHttpActionResult GetProducts() => Ok(db.Products.ToList());

    [HttpGet]
    public IHttpActionResult GetProduct(int id) { /* ... */ }

    [HttpPost]
    public IHttpActionResult PostProduct(Product model) { /* ... */ }

    [HttpPut]
    public IHttpActionResult PutProduct(int id, Product model) { /* ... */ }

    [HttpDelete]
    public IHttpActionResult DeleteProduct(int id) { /* ... */ }
}
```

## 9. Luồng hoạt động hệ thống

### 9.1. Luồng khách hàng

1. Khách truy cập trang chủ; hệ thống tải menu danh mục từ CSDL và danh sách sản phẩm.
2. Khách chọn danh mục hoặc nhập từ khóa để lọc/tìm sản phẩm.
3. Khách mở trang chi tiết, chọn size – màu – số lượng.
4. Hệ thống kiểm tra variant/tồn kho và thêm `CartItem` vào `Session["Cart"]`.
5. Khách xem giỏ; cập nhật số lượng, xóa từng dòng hoặc xóa toàn bộ.
6. Khi checkout, nếu chưa đăng nhập thì chuyển sang Login; sau login quay lại quy trình đặt hàng.
7. Khách xác nhận thông tin nhận hàng; hệ thống tạo `Orders` và `OrderDetails` rồi `SaveChanges()`.
8. Đặt thành công thì xóa Session Cart và hiển thị trang xác nhận; khách xem lịch sử đơn.

### 9.2. Luồng Admin

Đăng nhập (Role=Admin) → Dashboard → CRUD Category/Product/Variant/Image → theo dõi và cập nhật trạng thái đơn
→ quản lý Promotion → xem Revenue (đơn Completed) → kiểm thử Web API.

## 10. Kế hoạch theo tuần và mốc môn học

| Tuần | Công việc | Phụ trách | Deliverable |
|---|---|---|---|
| 1 | Chốt đề tài, thành viên, repo, quy tắc làm việc | Cùng làm | Project charter + repo |
| 2 | Phân tích actor/use case, wireframe, chức năng, tên bảng | Cùng làm | Use case + wireframe + backlog |
| 3 | Chốt ERD, SQL script, tạo MVC solution, baseline layout | Sử chính; Huy/Tú hỗ trợ | DB v1 + solution build |
| 4 | Customer layout + admin layout + EF model + seed data | Huy/Sử/Tú theo module | 2 layout + EF chạy |
| **5** | **BÁO CÁO LẦN 1:** phân công + giao diện thiết kế chức năng | Cùng làm | Bảng phân công + screenshot/wireframe |
| 6 | Category/Product list + CRUD Category/Product cơ bản | Huy/Tú; Sử hỗ trợ DB | Product/Category chạy DB |
| 7 | Search/filter/sort + validation + chi tiết sản phẩm | Huy chính | Customer product flow |
| 8 | Register/Login/Session + phân quyền Admin | Sử chính; Huy UI | Auth ổn định |
| 9 | Variant/Images + related products + admin product hoàn chỉnh | Huy/Tú | Detail + CRUD nâng cao |
| 10 | Cart Add/Update/Delete/Clear + cart count header | Sử chính; Huy UI | Cart hoàn chỉnh |
| 11 | Checkout/Order/History + Admin Order | Sử/Tú | Đặt hàng lưu DB |
| **12** | **BÁO CÁO LẦN 2:** diagram CSDL + quản lý danh mục + mua hàng | Cùng làm | ERD + purchase flow demo |
| 13 | Promotion + Revenue + Web API + regression test | Tú chính; Sử integration | Admin nâng cao + API |
| **14** | **BÁO CÁO LẦN 3:** quản lý bán hàng + nộp report + phần mềm | Cùng làm | Release candidate + báo cáo |
| Dự phòng | Fix bug, seed lại DB, luyện demo/vấn đáp | Cùng làm | Bản nộp sạch, có backup |

### Gate kiểm soát tiến độ

| Gate | Điều kiện phải đạt |
|---|---|
| Gate A — sau Tuần 4 | Solution build; DB import được; EF query được; customer/admin layout chạy |
| Gate B — trước Tuần 8 | Product list/search/detail chạy bằng DB; CRUD category/product cơ bản |
| Gate C — trước Tuần 12 | Login + Cart + Checkout + Order lưu DB; demo mua hàng end-to-end |
| Gate D — trước Tuần 14 | Admin order + promotion + revenue + API + report + test regression |

## 11. Rủi ro và phương án

| ID | Rủi ro | Mức | Phương án |
|---|---|---|---|
| R1 | Schema DB thay đổi liên tục | Cao | Sử chốt DB v1 sớm; thay đổi dùng script migration/manual change log |
| R2 | Merge conflict nhiều | Cao | Phân file/module; merge nhỏ thường xuyên; không cùng sửa một controller lớn |
| R3 | Cart/Order lỗi cuối kỳ | Cao | Làm từ Tuần 10, test end-to-end trước Tuần 12 |
| R4 | Ảnh/path lỗi khi chạy máy khác | Trung bình | Đường dẫn tương đối trong `Content/images`; test clone máy khác |
| R5 | Connection string khác máy | Trung bình | README hướng dẫn; giữ tên connection thống nhất |
| R6 | Một thành viên chậm | Cao | Gate hàng tuần; chia task nhỏ; integration tiếp quản interface đã định |
| R7 | UI đẹp nhưng thiếu rubric | Cao | Checklist rubric là Definition of Done; review mỗi tuần |
| R8 | Admin không phân quyền | Cao | Kiểm tra Session/Role ngay từ Tuần 8 |
| R9 | Dữ liệu demo nghèo | Thấp | Seed sớm 18 sản phẩm, nhiều category/variant |
| R10 | Demo Internet/Git lỗi | Trung bình | Demo local; mang zip + SQL backup; không phụ thuộc mạng |

## 12. Definition of Done

Một chức năng được coi là **Done** khi:

- Code đã build không lỗi.
- Chạy với dữ liệu thật từ DB (trừ phần thuần UI).
- Có xử lý null / ID không tồn tại / validation cơ bản.
- Dùng layout chung, không vỡ bố cục.
- Có testcase hoặc đã chạy smoke test liên quan.
- Đã commit/push và merge vào `develop`.
- Có screenshot hoặc ghi chú để dùng cho báo cáo nếu là chức năng chính.

Checklist đầy đủ trước báo cáo Tuần 12 / trước nộp Tuần 14: [`KIEM_THU.md`](KIEM_THU.md) mục 4–5.

## Phụ lục A. Route/Action đề xuất

| HTTP | URL | Controller/Action | Mục đích |
|---|---|---|---|
| GET | `/` | `Home/Index` | Trang chủ |
| GET | `/Product` | `Product/Index` | Danh sách sản phẩm |
| GET | `/Product?categoryId=1` | `Product/Index` | Lọc danh mục |
| GET | `/Product/Search?keyword=ao` | `Product/Search` hoặc `Index` | Tìm kiếm |
| GET | `/Product/Detail/5` | `Product/Detail` | Chi tiết |
| GET/POST | `/Account/Register` | `Account/Register` | Đăng ký |
| GET/POST | `/Account/Login` | `Account/Login` | Đăng nhập |
| GET | `/Account/Logout` | `Account/Logout` | Đăng xuất |
| POST/GET | `/Cart/AddToCart` | `Cart/AddToCart` | Thêm giỏ |
| GET | `/Cart` | `Cart/Index` | Xem giỏ |
| POST | `/Cart/Update` | `Cart/Update` | Cập nhật |
| GET/POST | `/Cart/Remove` | `Cart/Remove` | Xóa dòng |
| GET/POST | `/Order/Checkout` | `Order/Checkout` | Xác nhận đặt hàng |
| GET | `/Order/History` | `Order/History` | Lịch sử |
| GET | `/AdminProduct` | `AdminProduct/Index` | QL sản phẩm |
| GET/POST | `/AdminProduct/Create` | `AdminProduct/Create` | Thêm SP |
| GET/POST | `/AdminProduct/Edit/5` | `AdminProduct/Edit` | Sửa SP |
| POST/GET | `/AdminProduct/Delete/5` | `AdminProduct/Delete` | Xóa SP |
| GET | `/AdminOrder` | `AdminOrder/Index` | QL đơn |
| POST | `/AdminOrder/UpdateStatus` | `AdminOrder/UpdateStatus` | Đổi trạng thái |
| GET | `/Report/Revenue` | `Report/Revenue` | Doanh thu |

## Phụ lục B. Web API đề xuất

| Method | Endpoint | Nghiệp vụ | Kết quả |
|---|---|---|---|
| GET | `/api/products` | Danh sách sản phẩm | 200 + JSON/XML list |
| GET | `/api/products/5` | Chi tiết sản phẩm id=5 | 200 hoặc 404 |
| POST | `/api/products` | Tạo sản phẩm | 201/200 hoặc BadRequest |
| PUT | `/api/products/5` | Cập nhật sản phẩm | 200/204 hoặc 404 |
| DELETE | `/api/products/5` | Xóa/disable sản phẩm | 200/204 hoặc 404 |

Kiểm thử POST/PUT/DELETE bằng Postman hoặc công cụ gửi request tương đương.

## Phụ lục C. Nguồn tài liệu căn cứ

| Tài liệu | Vai trò |
|---|---|
| `Decuong_LapTrinhWeb(1).pdf` | Đề cương học phần: CLO, phân bố nội dung, đánh giá, quy định |
| `BaiGiang_LapTrinhWeb (2)_1(1).pdf` | Bài giảng/thực hành: MVC, ADO.NET, EF, cart/order, Web API |
| `3.ThangDiemdanhGia(1).pdf` | Thang điểm chi tiết — dùng làm checklist chức năng |
| `KeHoach_BTNhom(1).pdf` | Mốc Tuần 5/12/14; nội dung báo cáo và bàn giao |

> Ghi chú: rubric cuối đề cương có vài dòng dùng thuật ngữ WPF/MVVM, trong khi mô tả học phần, bài giảng và
> thang điểm đều tập trung ASP.NET MVC/Web. Kế hoạch này bám phần Web; nếu giảng viên yêu cầu khác,
> nhóm cập nhật baseline sau khi xác nhận.
