# KidsFashionStore — Website quản lý mặt hàng thời trang trẻ em

> Đồ án môn **Lập trình Web** — Trường ĐH Công Thương TP.HCM (HUIT), Khoa Công nghệ Thông tin.
> Phiên bản kế hoạch: 1.0 · Baseline: 15/09/2026

Website bán và quản lý mặt hàng thời trang trẻ em gồm 2 khu vực:

- **Khách hàng:** xem danh mục, tìm kiếm/lọc/sắp xếp sản phẩm, xem chi tiết (size – màu – tồn kho),
  giỏ hàng, đăng ký/đăng nhập, đặt hàng, xem lịch sử đơn.
- **Admin:** CRUD danh mục/sản phẩm/biến thể/hình ảnh, quản lý đơn hàng và trạng thái,
  khuyến mãi, thống kê doanh thu, Web API sản phẩm.

## Thành viên & phân công

| Thành viên | MSSV | GitHub | Module chính |
|---|---|---|---|
| Trần Nhật Huy | 2001240182 | *(cập nhật)* | Customer UI: layout, home, product list/search/filter, product detail, account UI, CSS |
| Hồ Quốc Sử | 2001240412 | [@hoquocsu1902](https://github.com/hoquocsu1902) | Database/ERD, EF Model, Auth + Session, Cart, Checkout/Order, Integration |
| Võ Ngọc Tú | 2001240534 | *(cập nhật)* | Admin: layout/dashboard, CRUD Category/Product/Variant/Image, Order management, Promotion, Revenue, Web API |

Chi tiết phân công, ma trận trách nhiệm và điểm giao tiếp: [`docs/PHAN_CONG.md`](docs/PHAN_CONG.md).

## Công nghệ

| Thành phần | Lựa chọn |
|---|---|
| Mô hình | ASP.NET MVC 5 — .NET Framework |
| Ngôn ngữ | C# |
| Giao diện | Razor (.cshtml) + Bootstrap + CSS + JavaScript/jQuery |
| CSDL | Microsoft SQL Server |
| ORM | Entity Framework (Database First) + LINQ |
| Trạng thái | Session cho User / Role / Cart |
| API | ASP.NET Web API — Product API (GET/POST/PUT/DELETE) |
| Quản lý source | Git + GitHub |

## Cấu trúc repository

```
kids-fashion-store/
├── README.md                  # file này
├── CONTRIBUTING.md            # quy trình git, branch, commit, review
├── .gitignore / .gitattributes / .editorconfig
├── .github/                   # issue template, pull request template, CODEOWNERS
├── Database/
│   ├── QL_ThoiTrangTreEm.sql  # script tạo DB + dữ liệu mẫu (chạy lại được)
│   └── README.md              # hướng dẫn import database
├── docs/
│   ├── CHARTER.md             # project charter (deliverable Tuần 1)
│   ├── KE_HOACH_DU_AN.md      # tóm tắt kế hoạch đồ án (baseline v1.0)
│   ├── PHAN_CONG.md           # phân công chi tiết + ma trận trách nhiệm
│   ├── THIET_KE_DATABASE.md   # thiết kế CSDL, ERD, quy ước dữ liệu, change log
│   └── KIEM_THU.md            # smoke test, test biên, checklist, kịch bản demo
└── src/                       # solution Visual Studio (nhóm tạo trên máy Windows)
    ├── KidsFashionStore.sln
    └── KidsFashionStore/
        ├── App_Start/  Controllers/  Models/  Views/
        ├── Content/    Scripts/      Global.asax  Web.config
```

> **Lưu ý:** thư mục `src/` hiện chỉ có hướng dẫn. Solution ASP.NET MVC 5 phải được tạo bằng
> Visual Studio trên Windows (macOS/Linux không build được .NET Framework). Xem [`src/README.md`](src/README.md).

## Bắt đầu nhanh

### 1. Clone repo

```bash
git clone https://github.com/hoquocsu1902/kids-fashion-store.git
cd kids-fashion-store
git checkout develop        # nhánh làm việc chung, KHÔNG code trực tiếp trên main
```

### 2. Cài môi trường (máy Windows)

- Visual Studio 2022/2019 — workload **ASP.NET and web development** (có sẵn MVC 5, EF, Web API).
- SQL Server (Express / Developer) + SQL Server Management Studio (SSMS).
- Git for Windows.

### 3. Import database

Mở `Database/QL_ThoiTrangTreEm.sql` bằng SSMS và Execute (script tự tạo database + dữ liệu mẫu,
chạy lại được nhiều lần). Chi tiết: [`Database/README.md`](Database/README.md).

### 4. Mở solution và cấu hình

1. Mở `src/KidsFashionStore.sln` bằng Visual Studio.
2. Kiểm tra chuỗi kết nối trong `Web.config`:

   ```xml
   <connectionStrings>
     <add name="QL_ThoiTrangTreEmEntities"
          connectionString="Data Source=.;Initial Catalog=QL_ThoiTrangTreEm;Integrated Security=True"
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

   - SQL Server Express: đổi `Data Source=.` thành `Data Source=.\SQLEXPRESS`.
   - **Không** commit mật khẩu thật / connection string cá nhân lên repo public.
3. `Ctrl + F5` để chạy (Visual Studio tự restore NuGet packages).

### 5. Tài khoản demo

| Vai trò | Username | Password |
|---|---|---|
| Admin | `admin` | `admin123` |
| Khách hàng | `khachhang1` | `123456` |
| Khách hàng | `khachhang2` | `123456` |

> Mật khẩu lưu dạng **SHA-256 (hex, chữ hoa)** trong bảng `Users.PasswordHash`.
> SQL seed dùng `HASHBYTES('SHA2_256', <password>)`; C# dùng
> `BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "")`.
> Chỉ dùng mật khẩu ASCII để 2 cách băm cho cùng kết quả.

## Quy trình làm việc

| Nhánh | Mục đích |
|---|---|
| `main` | Bản ổn định dùng để demo/nộp (chỉ merge tại các Gate) |
| `develop` | Nhánh tích hợp thường xuyên — mọi feature merge vào đây |
| `feature/huy-customer` | Customer UI / product |
| `feature/su-cart-order` | DB / auth / cart / order / integration |
| `feature/tu-admin` | Admin / CRUD / order / promotion / API |

- Pull `develop` mới nhất trước khi bắt đầu mỗi task.
- Commit theo quy ước: `feat:`, `fix:`, `style:`, `refactor:`, `docs:`, `test:`, `chore:`.
- Mở Pull Request vào `develop`, không push trực tiếp vào `main`/`develop`.
- **Không force push** `main`/`develop`; **không merge** code đang lỗi build.
- Cuối mỗi buổi làm việc phải push branch của mình lên GitHub.

Chi tiết đầy đủ: [`CONTRIBUTING.md`](CONTRIBUTING.md).

## Tài liệu

| Tài liệu | Nội dung |
|---|---|
| [`docs/CHARTER.md`](docs/CHARTER.md) | Project charter: mục tiêu, phạm vi, vai trò, quy tắc nhóm |
| [`docs/KE_HOACH_DU_AN.md`](docs/KE_HOACH_DU_AN.md) | Kế hoạch đồ án: chức năng F01–F24, mốc tuần, gate, rủi ro, DoD |
| [`docs/PHAN_CONG.md`](docs/PHAN_CONG.md) | Phân công chi tiết 3 thành viên + điểm giao tiếp bắt buộc |
| [`docs/THIET_KE_DATABASE.md`](docs/THIET_KE_DATABASE.md) | 9 bảng, quan hệ, quy ước dữ liệu, changelog schema |
| [`docs/ERD.md`](docs/ERD.md) | ERD v1 (Mermaid) + quyết định thiết kế + đối chiếu rubric |
| [`docs/KIEM_THU.md`](docs/KIEM_THU.md) | Smoke test ST01–ST15, test biên, checklist tuần 12/14, kịch bản demo |
| [`docs/Ke_hoach_do_an_Lap_trinh_Web.pdf`](docs/Ke_hoach_do_an_Lap_trinh_Web.pdf) | Bản kế hoạch đồ án gốc (PDF) của nhóm |

## Tiến độ

Theo dõi tiến độ tại [Issues](../../issues) và [Milestones](../../milestones) của repo.
Mốc bắt buộc của môn: **báo cáo lần 1 (Tuần 5)**, **lần 2 (Tuần 12)**, **lần 3/nộp (Tuần 14)**.
