# KẾ HOẠCH KIỂM THỬ — KidsFashionStore

> Mỗi lần merge lớn vào `develop` (đặc biệt tại các Gate), người merge phải chạy tối thiểu bộ **smoke test**
> liên quan và ghi kết quả vào Pull Request.
> Owner chạy test end-to-end: **Sử**. Người khác review/test theo ma trận trong [`PHAN_CONG.md`](PHAN_CONG.md).

---

## 1. Smoke test bắt buộc mỗi lần merge lớn

| ID | Test | Kết quả mong đợi | Người chạy chính |
|---|---|---|---|
| ST01 | Home mở được | 200 + giao diện đầy đủ, menu lấy DB | Huy |
| ST02 | Search sản phẩm | Kết quả đúng keyword; rỗng không crash | Huy |
| ST03 | Filter category | Đúng danh mục và link hoạt động | Huy |
| ST04 | Product detail | Đúng id; id sai → 404/điều hướng hợp lý | Huy |
| ST05 | Login đúng/sai | Đúng → tạo Session; sai → thông báo | Sử |
| ST06 | Add to cart | Thêm đúng variant + quantity | Sử |
| ST07 | Update/Delete cart | Tổng tiền/tổng số lượng cập nhật đúng | Sử |
| ST08 | Checkout chưa login | Redirect Login | Sử |
| ST09 | Checkout hợp lệ | Orders + OrderDetails được tạo | Sử |
| ST10 | Admin authorization | Customer không vào được trang Admin | Tú |
| ST11 | CRUD Product | Create/Edit/Delete/List đúng DB | Tú |
| ST12 | Order status | Admin cập nhật và hiển thị đúng | Tú |
| ST13 | Promotion | Ngày/% hợp lệ, hiển thị đúng trạng thái | Tú |
| ST14 | Revenue | Tính từ đơn Completed | Tú |
| ST15 | Web API | GET/POST/PUT/DELETE trả kết quả hợp lệ | Tú |

**Cách ghi kết quả** vào PR/Issue: `ST05 PASS (ảnh chụp)` hoặc `ST07 FAIL — mô tả lỗi + issue kèm theo`.

## 2. Test dữ liệu biên

- [ ] Số lượng giỏ = 0, âm hoặc lớn hơn tồn kho → chặn + thông báo.
- [ ] Keyword rỗng hoặc chỉ chứa khoảng trắng → không crash, hiển thị danh sách mặc định.
- [ ] ProductId/CategoryId/OrderId không tồn tại → 404 hoặc điều hướng hợp lý.
- [ ] Username/email trùng → báo lỗi tại form; mật khẩu rỗng; email sai định dạng.
- [ ] Promotion % < 0 hoặc > 100; StartDate > EndDate → chặn khi submit.
- [ ] Xóa category đang có product; xóa product đã tồn tại trong đơn cũ → cảnh báo/chặn.
- [ ] Truy cập URL admin khi chưa login hoặc Role = Customer → redirect/deny.
- [ ] Cart Session hết hạn hoặc null khi checkout → xử lý không crash.
- [ ] Variant có `StockQuantity = 0` (seed sẵn: *Áo thun bé trai tay dài / 5-6T*) → không cho thêm vào giỏ.

## 3. Test giao diện

- [ ] Kiểm tra Chrome/Edge ở desktop; responsive tối thiểu ở phone / tablet / desktop.
- [ ] Không tràn bảng/text; ảnh kích thước thống nhất; nút có trạng thái hover rõ.
- [ ] Form hiển thị validation gần trường nhập; thông báo lỗi/thành công dễ hiểu.
- [ ] Layout khách và admin không lẫn menu/quyền.

## 4. Checklist trước báo cáo Tuần 12

- [ ] ERD khớp SQL Server thật
- [ ] Menu 2 cấp lấy từ DB
- [ ] Product list/search/filter chạy
- [ ] Product detail chạy
- [ ] Login/Session chạy
- [ ] Cart Add/Update/Delete/Clear chạy
- [ ] Checkout lưu Order + OrderDetail
- [ ] Có dữ liệu mẫu đủ demo
- [ ] Có screenshot/report lần 2

## 5. Checklist trước nộp Tuần 14

- [ ] Customer flow không lỗi
- [ ] Admin phân quyền
- [ ] CRUD Category / Product
- [ ] Variant/Image
- [ ] Order management
- [ ] Promotion / Revenue
- [ ] Web API
- [ ] Responsive cơ bản
- [ ] Không hard-code dữ liệu demo quan trọng
- [ ] SQL script chạy lại được
- [ ] README có tài khoản demo
- [ ] Báo cáo đúng nội dung
- [ ] Ảnh màn hình đầy đủ
- [ ] Source build ở máy thứ hai (thành viên khác clone + cấu hình DB + chạy được)
- [ ] Backup zip + database

> **Chuẩn bàn giao:** không nộp phiên bản chỉ chạy được trên máy một thành viên. Ít nhất một thành viên khác
> phải clone source, cấu hình DB và chạy được luồng demo chính trước ngày nộp.

## 6. Ảnh minh chứng cần chụp

Home + menu 2 cấp · Product list + search/filter · Product detail + size/màu + related · Register/Login và trạng
thái đã đăng nhập · Cart trước/sau update · Checkout + bản ghi Orders/OrderDetails trong SQL Server ·
Admin Product/Category CRUD · Admin Order status · Promotion và Revenue · Web API GET + một POST/PUT/DELETE qua
Postman · ERD/diagram database.

## 7. Kịch bản demo (8–12 phút)

| Thời lượng | Nội dung |
|---|---|
| 0:00–1:00 | Giới thiệu đề tài, thành viên, kiến trúc |
| 1:00–3:00 | Khách: Home → category → search → detail |
| 3:00–5:00 | Chọn variant → Cart → update/delete → checkout |
| 5:00–6:00 | Login → đặt hàng → xác nhận lưu DB |
| 6:00–8:30 | Admin: Product/Category CRUD → Order status |
| 8:30–9:30 | Promotion + Revenue |
| 9:30–10:30 | Web API GET/POST hoặc GET/PUT |
| 10:30–12:00 | ERD, phân công, kết luận và chuẩn bị vấn đáp |

## 8. Definition of Done cho một chức năng

- [ ] Build không lỗi
- [ ] Chạy với dữ liệu thật từ DB
- [ ] Xử lý null / ID không tồn tại / validation cơ bản
- [ ] Dùng layout chung, không vỡ bố cục
- [ ] Có testcase hoặc đã chạy smoke test liên quan
- [ ] Đã commit/push + merge vào `develop` qua PR
- [ ] Có screenshot/ghi chú cho báo cáo nếu là chức năng chính
