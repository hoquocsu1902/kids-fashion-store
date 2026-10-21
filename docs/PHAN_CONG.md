# PHÂN CÔNG CHI TIẾT — 3 THÀNH VIÊN

> Nguyên tắc: mỗi người có module chính rõ ràng nhưng vẫn review chéo.
> **Hồ Quốc Sử phụ trách integration** để giảm rủi ro xung đột database/session;
> **Trần Nhật Huy** tập trung customer UI; **Võ Ngọc Tú** tập trung admin và nghiệp vụ quản lý.

---

## 1. Trần Nhật Huy — 2001240182 (Customer UI)

| Nhiệm vụ | File/khu vực | Đầu ra nghiệm thu |
|---|---|---|
| Layout khách hàng | `Views/Shared/_Layout.cshtml`, `_CategoryMenu.cshtml` | Header/footer/menu 2 cấp responsive, link đúng route |
| Trang chủ | `HomeController` + `Views/Home/Index` | Sản phẩm mới/nổi bật, điều hướng |
| Danh sách sản phẩm | `Views/Product/Index` | Card sản phẩm, filter category, search UI, sort UI |
| Chi tiết sản phẩm | `Views/Product/Detail` | Ảnh, mô tả, price, size/color, nút Add to Cart, related products |
| Account UI | `Views/Account/Login`, `Register` | Form rõ ràng, validation message, responsive |
| CSS khách hàng | `Content/site.css` | Màu sắc hài hòa, hover, spacing, phone/tablet/desktop |
| Ảnh sản phẩm | `Content/images/products/` | Ảnh đúng naming convention, kích thước thống nhất |
| Báo cáo phần khách | Ảnh màn hình + mô tả | Phân tích UI, sản phẩm, search/filter/detail |

**Lưu ý cho Huy:**
- Không chỉnh trực tiếp schema DB khi chưa báo Sử.
- Khi cần dữ liệu mới, tạo issue/ghi rõ field cần thêm để thống nhất.
- Mỗi trang phải chạy với dữ liệu thật, không để card mẫu hard-code ở bản nộp.
- Hoàn thành customer flow đến nút **Add to Cart** trước khi tích hợp Checkout.

## 2. Hồ Quốc Sử — 2001240412 (DB + Integration)

| Nhiệm vụ | File/khu vực | Đầu ra nghiệm thu |
|---|---|---|
| Phân tích và ERD | Database design | Chốt bảng, PK/FK, kiểu dữ liệu, script tạo DB + seed |
| Entity Framework | `Models/*.edmx` | Tạo/update model, kiểm tra navigation properties |
| Authentication backend | `AccountController` + Session | Login/logout/register logic, Session User/Role |
| Cart | `CartItem` + `CartController` | Add/Update/Delete/Clear/Totals, kiểm tra variant/tồn kho |
| Checkout/Order | `OrderController` | Tạo Order/OrderDetail, SaveChanges, xóa cart sau thành công |
| Order history | `OrderController` + `Views/Order` | Lịch sử và chi tiết đơn theo user |
| Integration | `develop` branch | Merge, resolve conflict, route, Web.config, kiểm thử end-to-end |
| Báo cáo kỹ thuật | ERD/MVC/Cart/Order | Viết phần kiến trúc, database, luồng hệ thống; chuẩn hóa report |

**Lưu ý cho Sử:**
- Giữ một file SQL chuẩn trong `/Database` và cập nhật khi schema thay đổi.
- Trước khi merge phải backup DB/schema và kiểm tra solution build thành công.
- Không để Cart phụ thuộc trực tiếp vào View; logic Cart nằm ở model/controller.
- Kiểm tra tất cả luồng dữ liệu giao nhau giữa customer và admin.

## 3. Võ Ngọc Tú — 2001240534 (Admin + Nghiệp vụ quản lý)

| Nhiệm vụ | File/khu vực | Đầu ra nghiệm thu |
|---|---|---|
| Admin layout/dashboard | `_AdminLayout` + Dashboard | Sidebar/menu quản trị, hiển thị tổng quan |
| CRUD Category | `AdminCategoryController` | List/Create/Edit/Delete/Details; hỗ trợ parent category |
| CRUD Product | `AdminProductController` | List/Create/Edit/Delete; category, price, image |
| Variant/Image | AdminProduct hoặc controller phụ | Size/màu/tồn kho, hình chính/phụ, radio/checkbox nếu dùng |
| Order management | `AdminOrderController` | Danh sách, detail, cập nhật status |
| Promotion | `PromotionController` | CRUD, ngày bắt đầu/kết thúc, % giảm |
| Revenue | `ReportController` | Thống kê doanh thu từ đơn Completed |
| Web API | `ProductApiController` | GET/POST/PUT/DELETE + test bằng Postman/browser |
| Báo cáo admin | Ảnh màn hình + mô tả | CRUD, order, promotion, revenue, API |

## 4. Ma trận trách nhiệm (Chính / Hỗ trợ / Review-Test)

| Hạng mục | Huy | Sử | Tú |
|---|---|---|---|
| Database/ERD | Hỗ trợ | **Chính** | Hỗ trợ |
| EF Model | Review/Test | **Chính** | Review/Test |
| Customer layout | **Chính** | Hỗ trợ | Review/Test |
| Product list/search/filter | **Chính** | Hỗ trợ | Review/Test |
| Product detail/related | **Chính** | Hỗ trợ | Review/Test |
| Register/Login UI | **Chính** | **Chính** | Review/Test |
| Session/Auth backend | Hỗ trợ | **Chính** | Review/Test |
| Cart | Hỗ trợ | **Chính** | Review/Test |
| Checkout/Order | Hỗ trợ | **Chính** | Hỗ trợ |
| Admin layout | Review/Test | Hỗ trợ | **Chính** |
| CRUD Category | Review/Test | Hỗ trợ | **Chính** |
| CRUD Product | Review/Test | Hỗ trợ | **Chính** |
| Order management | Review/Test | Hỗ trợ | **Chính** |
| Promotion/Revenue | Review/Test | Hỗ trợ | **Chính** |
| Web API | Review/Test | Hỗ trợ | **Chính** |
| Integration build | Hỗ trợ | **Chính** | Hỗ trợ |
| Test end-to-end | Review/Test | **Chính** | Review/Test |
| Báo cáo cuối | **Chính** | **Chính** | **Chính** |

## 5. Điểm giao tiếp bắt buộc (không được vi phạm)

| # | Giao tiếp | Quy ước |
|---|---|---|
| 1 | Product/Category schema | Huy và Tú **chỉ sử dụng sau khi Sử chốt** tên field/quan hệ |
| 2 | Product Detail → Cart | Huy cung cấp `variantId` + `quantity`; Sử xử lý logic Cart |
| 3 | Checkout → Admin Order | Sử tạo đúng `Orders`/`OrderDetails`; Tú đọc và cập nhật trạng thái |
| 4 | Promotion → Product display | Tú định nghĩa dữ liệu promotion; Huy cập nhật UI giá giảm khi được tích hợp |
| 5 | Web API | Dùng **cùng EF context/schema**, không tạo database thứ hai |
| 6 | Routing / Web.config | **Mọi thay đổi phải thông báo trước khi merge** |

## 6. Ma trận bám tiêu chí chấm điểm

| Tiêu chí | Owner | Chức năng | Bằng chứng nghiệm thu |
|---|---|---|---|
| Layout liên kết chức năng | Huy | F01 | `_Layout` chạy tất cả link chính |
| Menu 2 cấp | Huy + Sử | F02 | Dữ liệu lấy từ CSDL |
| Giao diện màu sắc hài hòa | Huy | F01/F24 | Responsive + CSS/hover |
| Đăng ký | Huy + Sử | F09 | Tạo customer hợp lệ |
| Đăng nhập | Huy + Sử | F10 | Session user/role |
| Tìm kiếm | Huy | F05 | Tên gần đúng |
| Hiển thị danh sách SP | Huy | F03 | Card DB |
| CSS tô màu/chữ/hover | Huy | F24 | UI nhất quán |
| Lọc theo loại | Huy | F04 | CategoryId |
| Chi tiết SP – hiển thị | Huy | F07 | Thông tin đầy đủ |
| Chi tiết SP – định dạng | Huy | F07/F24 | Ảnh/text rõ |
| Chi tiết SP – bổ sung | Huy + Sử | F07 | Variant/tồn kho |
| SP liên quan | Huy | F08 | Cùng danh mục |
| Giỏ hàng chọn mua | Sử | F11/F12 | 1,n; update/delete |
| Đặt hàng | Sử | F13/F14 | Confirm + lưu DB |
| Bảng SP CRUD | Tú | F17 | Create/Edit/Delete |
| Hình/loại | Tú | F17/F18 | Category + image |
| Radio/checkbox | Tú | F18 | Dùng cho biến thể/active |
| Quản lý cập nhật đơn | Tú | F19 | Status |
| Quản lý khuyến mãi | Tú | F20 | CRUD |
| Thống kê doanh thu | Tú | F21 | Completed orders |
| Bảng loại CRUD | Tú | F16 | Create/Edit/Delete/List |
| Báo cáo đúng quy định | Cả nhóm | – | Không lỗi chính tả; có màn hình thực thi |
| Làm việc nhóm | Cả nhóm | – | Phân công + lịch sử commit + review |
