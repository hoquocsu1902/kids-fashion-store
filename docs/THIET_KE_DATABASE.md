# THIẾT KẾ CƠ SỞ DỮ LIỆU — QL_ThoiTrangTreEm

> Owner: **Hồ Quốc Sử**. Chỉ Sử được sửa schema. Thành viên khác cần field mới → tạo issue.
> Script chuẩn: [`../Database/QL_ThoiTrangTreEm.sql`](../Database/QL_ThoiTrangTreEm.sql) (chạy lại được nhiều lần).
> Bản vẽ ERD: [`ERD.md`](ERD.md) (Mermaid) · [`ERD.dbml`](ERD.dbml) (dán vào dbdiagram.io).
> Phiên bản schema: **v1.0**

---

## 1. Sơ đồ quan hệ (ERD logic)

> Sơ đồ đầy đủ kèm thuộc tính từng bảng: [`ERD.md`](ERD.md).

```
Users ──1─N──> Orders ──1─N──> OrderDetails ──N─1──> ProductVariants ──N─1──> Products
                                                                                │
Categories ──1─N──> Products ──1─N──> ProductImages                              │
    │                                                                           │
    └── tự tham chiếu (ParentId)                                                │
                                                                                │
Promotions ──N─N──> ProductPromotions <──N─N── Products ────────────────────────┘

Luồng: Users(1) → Orders(N) → OrderDetails(N) → ProductVariants(N) → Products(1)
```

## 2. Danh sách bảng

| # | Bảng | Khóa | Thuộc tính chính | Quan hệ |
|---|---|---|---|---|
| 1 | `Users` | `UserId` PK | Username (unique), PasswordHash, FullName, Email (unique), Phone, Address, Role, IsActive | 1–N `Orders` |
| 2 | `Categories` | `CategoryId` PK | CategoryName, ParentId (FK tự thân), Description, IsActive | Tự tham chiếu + 1–N `Products` |
| 3 | `Products` | `ProductId` PK | ProductName, CategoryId (FK), Price, Description, MainImage, Gender, CreatedDate, IsActive | N–1 `Categories`; 1–N `ProductVariants`, `ProductImages` |
| 4 | `ProductVariants` | `VariantId` PK | ProductId (FK), Size, Color, StockQuantity | N–1 `Products`; 1–N `OrderDetails` |
| 5 | `ProductImages` | `ImageId` PK | ProductId (FK), ImageUrl, SortOrder | N–1 `Products` |
| 6 | `Orders` | `OrderId` PK | UserId (FK), OrderDate, ReceiverName, Phone, ShippingAddress, TotalAmount, Status, Note | N–1 `Users`; 1–N `OrderDetails` |
| 7 | `OrderDetails` | `OrderDetailId` PK | OrderId (FK), VariantId (FK), Quantity, UnitPrice | N–1 `Orders`; N–1 `ProductVariants` |
| 8 | `Promotions` | `PromotionId` PK | PromotionName, DiscountPercent, StartDate, EndDate, IsActive | N–N `Products` qua bảng nối |
| 9 | `ProductPromotions` | `ProductId` + `PromotionId` PK | Hai khóa ngoại | Bảng nối N–N |

## 3. Ràng buộc và quy ước dữ liệu

| Hạng mục | Quy ước |
|---|---|
| Khóa chính | `INT IDENTITY(1,1)`, tên `<Entity>Id` |
| Khóa ngoại | Tên trùng khóa chính tham chiếu: `CategoryId`, `UserId`, `VariantId`, `ProductId`, `OrderId`, `PromotionId` |
| Giá tiền | `DECIMAL(18,0)` cho giá VND — **không dùng `float`/`double`** |
| Ngày giờ | `DATETIME` (`OrderDate` mặc định `GETDATE()`), `DATE` cho ngày khuyến mãi |
| `Users.Role` | `CHECK IN ('Customer','Admin')`, mặc định `Customer` |
| `Orders.Status` | `CHECK IN ('Pending','Confirmed','Shipping','Completed','Cancelled')`, mặc định `Pending` |
| `Promotions.DiscountPercent` | `CHECK BETWEEN 0 AND 100`; `CHECK (EndDate >= StartDate)` |
| `ProductVariants` | `UNIQUE (ProductId, Size, Color)` — không tạo variant trùng; `StockQuantity >= 0` |
| `OrderDetails.Quantity` | `CHECK (Quantity > 0)` |
| `Users.PasswordHash` | `VARCHAR(64)` — SHA-256 hex chữ hoa (khớp `HASHBYTES('SHA2_256', ...)` trong SQL) |
| Mật khẩu demo | `admin` / `admin123`; `khachhang1`, `khachhang2` / `123456` |
| Ảnh | Đường dẫn tương đối: `~/Content/images/products/<slug>-01.jpg` |
| Xóa dữ liệu | Không xóa cứng sản phẩm/danh mục đã phát sinh đơn — dùng `IsActive = 0` |

## 4. Chỉ mục (index) đã tạo trong script

- `IX_Products_CategoryId` — lọc sản phẩm theo danh mục (F04).
- `IX_ProductVariants_ProductId` — tải biến thể theo sản phẩm (F07/F18).
- `IX_Orders_UserId` + `IX_Orders_Status_OrderDate` — lịch sử đơn (F15), doanh thu (F21), admin order (F19).
- `IX_OrderDetails_OrderId` — chi tiết đơn (F19).
- `IX_ProductImages_ProductId`, `IX_ProductPromotions_PromotionId`.

## 5. Dữ liệu mẫu (seed)

| Bảng | Số dòng | Ghi chú |
|---|---|---|
| Users | 3 | 1 Admin + 2 Customer |
| Categories | 8 | 2 danh mục cha (bé trai, bé gái) + 6 danh mục con |
| Products | 18 | Nhiều mức giá 99.000 – 329.000, có `Gender` Boy/Girl |
| ProductVariants | 54 | Mỗi sản phẩm 3 variant (size + màu), tồn kho 0–30 (có variant hết hàng để test) |
| ProductImages | 36 | 2 ảnh/sản phẩm (ảnh chính + ảnh phụ) |
| Promotions | 3 | Đang chạy, đang chạy, sắp chạy — test trạng thái F20 |
| ProductPromotions | 11 | Gán sản phẩm vào khuyến mãi |
| Orders | 5 | Đủ trạng thái: 2 Completed, 1 Shipping, 1 Pending, 1 Cancelled — demo doanh thu F21 |
| OrderDetails | 10 | Khớp `TotalAmount` của từng đơn |

> Ảnh sản phẩm (file `.jpg`) do Huy chuẩn bị theo naming convention ở `Content/images/products/`.

## 6. Change log schema

Mọi thay đổi schema phải được ghi vào đây **và** cập nhật trong `Database/QL_ThoiTrangTreEm.sql`.

| Phiên bản | Ngày | Người thực hiện | Thay đổi | Lý do |
|---|---|---|---|---|
| v1.0 | 15/09/2026 | Sử | Tạo 9 bảng + ràng buộc + seed dữ liệu mẫu | Baseline theo kế hoạch |
| | | | | |

**Quy trình đổi schema:**

1. Tạo issue mô tả field/bảng cần thêm và lý do.
2. Sử cập nhật `Database/QL_ThoiTrangTreEm.sql` (script phải chạy lại được từ đầu).
3. Ghi 1 dòng vào changelog trên.
4. Thông báo cả nhóm trong PR liên quan để update EF model (`.edmx`) và ViewModel.
5. Trước khi merge: backup DB và build lại solution.
