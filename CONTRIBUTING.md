# CONTRIBUTING — Quy trình làm việc nhóm KidsFashionStore

Tài liệu này áp dụng cho **cả 3 thành viên** (Huy, Sử, Tú). Mục tiêu: mỗi người làm độc lập trên
module của mình nhưng tích hợp không xung đột, lịch sử commit rõ ràng để dùng làm bằng chứng "làm việc nhóm"
khi chấm điểm.

---

## 1. Nhánh (branch)

| Nhánh | Vai trò | Ai được ghi |
|---|---|---|
| `main` | Bản ổn định để demo/nộp. Chỉ merge từ `develop` khi đạt Gate hoặc chuẩn bị báo cáo. | Không ai push trực tiếp |
| `develop` | Nhánh tích hợp. Là nhánh gốc để tạo feature branch. | Qua Pull Request |
| `feature/huy-customer` | Customer UI: layout, home, product list/detail, account UI, CSS. | Huy |
| `feature/su-cart-order` | DB/ERD, EF model, auth/session, cart, checkout/order, integration. | Sử |
| `feature/tu-admin` | Admin layout/CRUD, order management, promotion, revenue, Web API. | Tú |

Quy tắc đặt tên nhánh task phụ (nếu cần tách nhỏ): `feature/<owner>-<module>`,
ví dụ `feature/huy-search-filter`, `feature/tu-promotion`.

## 2. Quy trình mỗi task

```bash
# 1. Lấy code mới nhất
git checkout develop
git pull origin develop

# 2. Cập nhật nhánh module của mình
git checkout feature/huy-customer          # hoặc branch task riêng
git merge develop                          # xử lý conflict ngay tại đây nếu có

# 3. Code một chức năng nhỏ, chạy thử local (F5)
git status
git add <các file đã sửa>
git commit -m "feat: add product search by keyword"

# 4. Push lên GitHub
git push origin feature/huy-customer
```

5. Mở **Pull Request** vào `develop` (điền đầy đủ template PR).
6. Ít nhất **1 thành viên khác** review (comment hoặc approve) — ưu tiên người phụ trách module liên quan.
7. Sau khi merge, người phụ trách integration (Sử) hoặc người merge chạy **smoke test** liên quan
   (xem [`docs/KIEM_THU.md`](docs/KIEM_THU.md)).
8. Chỉ merge `develop` → `main` khi đạt Gate hoặc chuẩn bị demo/báo cáo.

## 3. Quy ước commit

Định dạng: `type: mô tả ngắn (tiếng Anh hoặc tiếng Việt không dấu đều được, thống nhất một kiểu)`.

| Type | Dùng khi |
|---|---|
| `feat` | Thêm chức năng mới |
| `fix` | Sửa lỗi |
| `style` | CSS, format, không đổi logic |
| `refactor` | Đổi cấu trúc code, không đổi hành vi |
| `docs` | Tài liệu, README, comment |
| `test` | Thêm/sửa testcase, smoke test |
| `chore` | Cấu hình, package, file không thuộc code |

Ví dụ:

```
feat: add product search by keyword
fix: correct cart total when quantity updated
style: responsive product card on tablet
docs: update README with demo accounts
test: add smoke test ST07 for cart update
```

Không commit kiểu `update`, `fix bug`, `abc`, `...`. Mỗi commit chỉ nên là **một thay đổi nhỏ, chạy được**.

## 4. Quy tắc không được vi phạm

- ❌ Không commit mật khẩu thật, connection string cá nhân, file `.secrets.config` (repo public!).
- ❌ Không force push `main`/`develop`.
- ❌ Không merge code đang lỗi build.
- ❌ Không đổi tên entity/table/bảng hàng loạt khi chưa thống nhất với Sử (người chốt schema).
- ❌ Không sửa trực tiếp schema DB (Sử cập nhật `Database/QL_ThoiTrangTreEm.sql` + changelog trong `docs/THIET_KE_DATABASE.md`).
- ❌ Không để thay đổi quan trọng chỉ nằm ở máy cá nhân — **cuối mỗi buổi làm phải push**.
- ❌ Không hard-code dữ liệu demo quan trọng ở bản nộp (card sản phẩm, danh mục phải lấy từ DB).

## 5. Quy tắc code

- Controller chỉ điều phối request/response; không viết HTML trong Controller.
- View nhận dữ liệu từ Model/ViewModel; **không truy vấn SQL trực tiếp trong View**.
- Action thay đổi dữ liệu: `[HttpPost]` + `@Html.AntiForgeryToken()`.
- Trang admin: kiểm tra `Session["Role"] == "Admin"` trước khi xử lý.
- Đặt tên theo quy ước trong [`docs/KE_HOACH_DU_AN.md`](docs/KE_HOACH_DU_AN.md) (mục 7.1):
  `PascalCase + Controller`, action `PascalCase`, entity danh từ số ít, khóa chính `<Entity>Id`.
- Dùng `decimal` cho tiền, không dùng `float`/`double`.
- Xử lý `null`, ID không tồn tại, validation cơ bản trước khi merge.

## 6. Cấu trúc file — ai sở hữu file nào

| Khu vực | Owner | Được sửa bởi |
|---|---|---|
| `Database/*.sql`, `docs/THIET_KE_DATABASE.md` | Sử | Chỉ Sử (người khác đề xuất qua issue) |
| `Views/Shared/_Layout.cshtml`, `_CategoryMenu.cshtml`, `Content/site.css` | Huy | Huy |
| `Controllers/AdminProductController`, `AdminCategoryController`, `AdminOrderController`, `PromotionController`, `ReportController`, `ProductApiController` | Tú | Tú |
| `Controllers/HomeController`, `ProductController`, `Views/Home`, `Views/Product`, `Views/Account` | Huy | Huy |
| `Controllers/AccountController`, `CartController`, `OrderController`, `Models/*.edmx` | Sử | Sử |
| `Views/Shared/_AdminLayout.cshtml`, `Views/Admin*` | Tú | Tú |
| `Web.config`, `App_Start/*` | Sử | Thông báo trước khi sửa |

Cần dữ liệu/field mới ngoài schema → **tạo issue**, không tự thêm cột.

## 7. Git cheat sheet

```bash
git branch -a                     # xem tất cả nhánh
git checkout develop && git pull  # lấy code mới
git log --oneline --graph -15     # xem lịch sử gần đây
git diff                          # xem thay đổi chưa add
git stash / git stash pop         # tạm cất thay đổi khi cần chuyển nhánh
git restore <file>                # hủy thay đổi chưa commit của file
git pull --rebase origin develop  # cập nhật nhánh feature theo develop
```

Khi conflict: **không tự ý chọn "accept theirs" toàn bộ** — mở file, xác nhận từng đoạn, build lại rồi commit.
Vướng mắc thì báo ngay trong nhóm, ưu tiên gọi Sử (integration).
