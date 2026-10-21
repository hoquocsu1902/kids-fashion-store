# Database — QL_ThoiTrangTreEm

Script chuẩn: [`QL_ThoiTrangTreEm.sql`](QL_ThoiTrangTreEm.sql) · Schema v1.0 · Owner: **Sử**

## 1. Import database

**Cách 1 — SSMS (khuyến nghị):**

1. Mở SQL Server Management Studio, kết nối SQL Server (`.`, `.\SQLEXPRESS` hoặc `localhost`).
2. `File → Open → File...` chọn `QL_ThoiTrangTreEm.sql`.
3. Nhấn `Execute (F5)`. Script tự tạo database, bảng, ràng buộc, chỉ mục và dữ liệu mẫu.
4. Kiểm tra: chạy các câu `SELECT COUNT(*)` ở cuối file (đã ghi kết quả mong đợi trong comment).

**Cách 2 — sqlcmd:**

```powershell
sqlcmd -S .\SQLEXPRESS -E -i "Database\QL_ThoiTrangTreEm.sql"
```

## 2. Đặc điểm script

- **Chạy lại được nhiều lần**: database chỉ tạo nếu chưa có; các bảng bị drop theo thứ tự phụ thuộc
  rồi tạo lại và seed lại. Phù hợp khi cần reset dữ liệu demo.
- Có đầy đủ **PK/FK, UNIQUE, CHECK, DEFAULT, INDEX**.
- Dữ liệu mẫu: 3 users, 8 categories, 18 products, 54 variants, 36 images, 3 promotions, 5 orders.

## 3. Tài khoản demo (seed)

| Vai trò | Username | Password | Ghi chú |
|---|---|---|---|
| Admin | `admin` | `admin123` | Role = Admin |
| Khách hàng | `khachhang1` | `123456` | Có 3 đơn (Completed/Shipping/Cancelled) |
| Khách hàng | `khachhang2` | `123456` | Có 2 đơn (Completed/Pending) |

Mật khẩu lưu dạng SHA-256 hex chữ hoa. SQL sinh bằng `CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'admin123'), 2)`.
Code C# phải dùng đúng cách băm tương ứng:

```csharp
public static string ToSha256(string input)
{
    using (var sha = System.Security.Cryptography.SHA256.Create())
    {
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", ""); // hex CHỮ HOA
    }
}
```

## 4. Connection string (Web.config)

```xml
<connectionStrings>
  <add name="QL_ThoiTrangTreEmEntities"
       connectionString="Data Source=.;Initial Catalog=QL_ThoiTrangTreEm;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

- SQL Server Express: `Data Source=.\SQLEXPRESS`.
- **Không** commit connection string có mật khẩu thật / tài khoản cá nhân lên repo public.

## 5. Ảnh sản phẩm

Seed dùng đường dẫn tương đối `~/Content/images/products/<slug>-01.jpg` và `-02.jpg`
(danh sách slug nằm trong script, mục 5.5). Cần copy file ảnh thật vào
`src/KidsFashionStore/Content/images/products/` đúng tên.

## 6. Quy trình đổi schema

1. Tạo issue mô tả thay đổi.
2. Sử cập nhật `QL_ThoiTrangTreEm.sql` (giữ khả năng chạy lại từ đầu).
3. Ghi changelog tại [`../docs/THIET_KE_DATABASE.md`](../docs/THIET_KE_DATABASE.md) mục 6.
4. Cập nhật EF model (`.edmx`) + ViewModel và thông báo nhóm trước khi merge.

> Trước khi merge thay đổi DB: backup schema + build lại solution (quy tắc nhóm).
