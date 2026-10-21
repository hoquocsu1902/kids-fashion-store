# PROJECT CHARTER — KidsFashionStore

> Deliverable Tuần 1: **"Chốt đề tài, thành viên, repo, quy tắc làm việc"**.
> Phiên bản: 1.0 · Ngày lập: 15/09/2026 · Người cập nhật: cả nhóm

---

## 1. Thông tin dự án

| Hạng mục | Nội dung |
|---|---|
| Tên đề tài | Website quản lý mặt hàng thời trang trẻ em |
| Tên solution | KidsFashionStore |
| Tên database | QL_ThoiTrangTreEm |
| Môn học | Lập trình Web (3 tín chỉ: 15 tiết lý thuyết, 60 tiết thực hành) |
| Đơn vị | Trường ĐH Công Thương TP.HCM — Khoa Công nghệ Thông tin |
| Repo | https://github.com/hoquocsu1902/kids-fashion-store |
| Thời lượng dự kiến | 14 tuần, 3 mốc báo cáo (Tuần 5, 12, 14) |

## 2. Mục tiêu

**Tổng quát:** xây dựng website quản lý và bán mặt hàng thời trang trẻ em với hai khu vực khách hàng và
quản trị, cho phép tìm kiếm – xem chi tiết – chọn biến thể – giỏ hàng – đặt hàng, và quản trị danh mục,
sản phẩm, đơn hàng, khuyến mãi, doanh thu.

**Định lượng (điều kiện thành công):**

- Hoàn thành tối thiểu **24 nhóm chức năng/tiêu chí** trong checklist rubric.
- **100% chức năng chính có testcase và bằng chứng chạy thực tế** (ảnh màn hình).
- **Không lỗi runtime** ở luồng demo chính: Home → Product → Detail → Cart → Login → Checkout → lưu đơn.
- Database đầy đủ khóa chính/khóa ngoại và dữ liệu mẫu đủ demo (≥ 2 danh mục cha, 4–6 danh mục con,
  15–20 sản phẩm, nhiều variant).
- Mỗi thành viên có ít nhất 1 module độc lập để trình bày khi vấn đáp.
- Bản chạy ổn định hoàn thành trước **Tuần 14**.
- Nộp kèm SQL script chạy lại được + README hướng dẫn + báo cáo PDF/DOCX + ảnh minh chứng.

## 3. Phạm vi

**Trong phạm vi:**

- Bán và quản lý quần áo/thời trang trẻ em; chưa triển khai thanh toán trực tuyến thật.
- Đặt hàng lưu trực tiếp vào SQL Server; trạng thái đơn do admin cập nhật.
- Sản phẩm có danh mục, giá, mô tả, hình ảnh, giới tính/nhóm đối tượng, biến thể size – màu – tồn kho.
- Giỏ hàng dùng Session; dữ liệu đơn chỉ lưu DB khi khách xác nhận.
- Web API CRUD sản phẩm (GET/POST/PUT/DELETE) để chứng minh nội dung Web API.

**Ngoài phạm vi:** thanh toán online, email, cloud storage, microservices, React/Next.js.

## 4. Thành viên & vai trò

| Thành viên | MSSV | GitHub | Vai trò chính | Module phụ trách |
|---|---|---|---|---|
| Trần Nhật Huy | 2001240182 | [@huytran995399](https://github.com/huytran995399) | Customer UI / Frontend | Layout khách, Home, Product list/search/filter/sort, Product detail + related, Account UI, CSS |
| Hồ Quốc Sử | 2001240412 | [@hoquocsu1902](https://github.com/hoquocsu1902) | Database + Integration | ERD/SQL, EF model, Auth + Session, Cart, Checkout/Order/History, merge & smoke test |
| Võ Ngọc Tú | 2001240534 | [@vongoctu08102006](https://github.com/vongoctu08102006) | Admin / Backend nghiệp vụ | Admin layout/dashboard, CRUD Category/Product/Variant/Image, Order management, Promotion, Revenue, Web API |

Chi tiết từng đầu việc: [`PHAN_CONG.md`](PHAN_CONG.md).

## 5. Cách làm việc nhóm

| Hạng mục | Quy định |
|---|---|
| Kênh liên lạc chính | Nhóm chat Zalo/Messenger của nhóm (thông báo task, blocker) |
| Kênh lưu vết công việc | GitHub Issues + Pull Request (bắt buộc cho mọi task) |
| Họp nhóm | 1 buổi/tuần (offline hoặc online), có ghi chú ngắn trong Issue tương ứng |
| Cập nhật tiến độ | Cuối mỗi buổi làm: commit + push; mỗi tuần: cập nhật trạng thái Issue |
| Tích hợp | Feature merge vào `develop` qua PR; `develop` → `main` tại các Gate |
| Backup | Sử giữ 1 bản zip + 1 bản SQL dump ở 2 nơi khác nhau từ Tuần 12 |

**Quy tắc bất di bất dịch** (xem đầy đủ tại [`../CONTRIBUTING.md`](../CONTRIBUTING.md)):

1. Không commit mật khẩu thật / connection string cá nhân (repo public).
2. Không force push `main`/`develop`; không merge code lỗi build.
3. Không đổi schema/tên bảng khi chưa thống nhất với Sử.
4. Cuối mỗi buổi làm phải push.
5. Mỗi PR phải có ít nhất 1 người review.

## 6. Mốc bàn giao

| Mốc | Nội dung | Deliverable |
|---|---|---|
| Tuần 5 | Báo cáo lần 1 | Bảng phân công + screenshot/wireframe + repo chạy |
| Tuần 12 | Báo cáo lần 2 | ERD + diagram CSDL + demo quản lý danh mục & mua hàng |
| Tuần 14 | Báo cáo lần 3 / nộp | Release candidate + báo cáo + ảnh minh chứng + backup |

**Gate kiểm soát:** Gate A (sau Tuần 4) → Gate B (trước Tuần 8) → Gate C (trước Tuần 12) → Gate D (trước Tuần 14).
Điều kiện cụ thể từng Gate: [`KE_HOACH_DU_AN.md`](KE_HOACH_DU_AN.md) mục 10.

## 7. Rủi ro chính

| ID | Rủi ro | Mức | Phương án |
|---|---|---|---|
| R1 | Schema DB thay đổi liên tục | Cao | Sử chốt DB v1 sớm; mọi thay đổi qua changelog + script migration |
| R2 | Merge conflict nhiều | Cao | Phân file/module rõ; merge nhỏ, thường xuyên; không ai sửa chung 1 controller lớn |
| R3 | Cart/Order lỗi cuối kỳ | Cao | Làm từ Tuần 10, test end-to-end trước Tuần 12 |
| R4 | Ảnh/path lỗi khi chạy máy khác | Trung bình | Dùng đường dẫn tương đối `~/Content/images/...`; test clone trên máy thứ 2 |
| R5 | Connection string khác máy | Trung bình | README hướng dẫn; thống nhất dùng Integrated Security |
| R6 | Một thành viên chậm | Cao | Gate hàng tuần; task nhỏ; người integration có thể tiếp quản interface đã định |
| R7 | UI đẹp nhưng thiếu rubric | Cao | Checklist rubric là Definition of Done; review mỗi tuần |
| R8 | Admin không phân quyền | Cao | Kiểm tra Session/Role ngay từ Tuần 8 |
| R9 | Dữ liệu demo nghèo | Thấp | Seed sớm 18 sản phẩm, 8 category, nhiều variant |
| R10 | Demo Internet/Git lỗi | Trung bình | Demo local; mang zip + SQL backup |

## 8. Cam kết của thành viên

- Hoàn thành phần việc đúng mốc, chủ động báo trước khi trễ hạn.
- Review chéo phần việc của nhau theo ma trận trách nhiệm.
- Không nhận điểm chung khi phần việc cá nhân chưa hoàn thành.
- Giữ repo gọn, commit rõ ràng vì lịch sử commit là bằng chứng "làm việc nhóm" khi chấm.
