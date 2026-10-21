/* =====================================================================
   ĐỒ ÁN LẬP TRÌNH WEB - WEBSITE QUẢN LÝ MẶT HÀNG THỜI TRANG TRẺ EM
   ---------------------------------------------------------------------
   File     : QL_ThoiTrangTreEm.sql
   Mô tả    : Tạo database + 9 bảng + ràng buộc + chỉ mục + dữ liệu mẫu
   Phiên bản: v1.0
   Owner    : Hồ Quốc Sử (2001240412)
   ---------------------------------------------------------------------
   HƯỚNG DẪN : Mở bằng SSMS -> Execute (F5). Script CHẠY LẠI ĐƯỢC:
               database chỉ tạo nếu chưa có, các bảng bị drop theo thứ tự
               phụ thuộc rồi tạo lại + seed lại dữ liệu mẫu.
   LƯU Ý     : Mọi thay đổi schema phải cập nhật file này + changelog trong
               docs/THIET_KE_DATABASE.md
   ===================================================================== */

SET NOCOUNT ON;
GO

/* ---------------------------------------------------------------------
   1. TẠO DATABASE
   --------------------------------------------------------------------- */
IF DB_ID(N'QL_ThoiTrangTreEm') IS NULL
BEGIN
    CREATE DATABASE QL_ThoiTrangTreEm;
END
GO

USE QL_ThoiTrangTreEm;
GO

/* ---------------------------------------------------------------------
   2. DROP BẢNG CŨ (theo thứ tự phụ thuộc khóa ngoại)
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.ProductPromotions', N'U') IS NOT NULL DROP TABLE dbo.ProductPromotions;
IF OBJECT_ID(N'dbo.OrderDetails',      N'U') IS NOT NULL DROP TABLE dbo.OrderDetails;
IF OBJECT_ID(N'dbo.Orders',            N'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID(N'dbo.ProductImages',     N'U') IS NOT NULL DROP TABLE dbo.ProductImages;
IF OBJECT_ID(N'dbo.ProductVariants',   N'U') IS NOT NULL DROP TABLE dbo.ProductVariants;
IF OBJECT_ID(N'dbo.Products',          N'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID(N'dbo.Promotions',        N'U') IS NOT NULL DROP TABLE dbo.Promotions;
IF OBJECT_ID(N'dbo.Categories',        N'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID(N'dbo.Users',             N'U') IS NOT NULL DROP TABLE dbo.Users;
GO

/* ---------------------------------------------------------------------
   3. TẠO BẢNG
   --------------------------------------------------------------------- */

-- 3.1. Users: tài khoản khách hàng / admin
CREATE TABLE dbo.Users
(
    UserId       INT IDENTITY(1,1) NOT NULL,
    Username     NVARCHAR(50)      NOT NULL,
    PasswordHash VARCHAR(64)       NOT NULL,   -- SHA-256 hex (chữ hoa)
    FullName     NVARCHAR(100)     NOT NULL,
    Email        VARCHAR(100)      NOT NULL,
    Phone        VARCHAR(20)       NULL,
    Address      NVARCHAR(255)     NULL,
    Role         VARCHAR(20)       NOT NULL CONSTRAINT DF_Users_Role     DEFAULT ('Customer'),
    IsActive     BIT               NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CreatedDate  DATETIME          NOT NULL CONSTRAINT DF_Users_Created  DEFAULT (GETDATE()),
    CONSTRAINT PK_Users          PRIMARY KEY (UserId),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email    UNIQUE (Email),
    CONSTRAINT CK_Users_Role     CHECK (Role IN ('Customer', 'Admin'))
);
GO

-- 3.2. Categories: danh mục 2 cấp (ParentId tự tham chiếu)
CREATE TABLE dbo.Categories
(
    CategoryId   INT IDENTITY(1,1) NOT NULL,
    CategoryName NVARCHAR(100)     NOT NULL,
    ParentId     INT               NULL,
    Description  NVARCHAR(255)     NULL,
    IsActive     BIT               NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT (1),
    CONSTRAINT PK_Categories         PRIMARY KEY (CategoryId),
    CONSTRAINT FK_Categories_Parent  FOREIGN KEY (ParentId) REFERENCES dbo.Categories (CategoryId)
);
GO

-- 3.3. Products
CREATE TABLE dbo.Products
(
    ProductId   INT IDENTITY(1,1) NOT NULL,
    ProductName NVARCHAR(200)     NOT NULL,
    CategoryId  INT               NOT NULL,
    Price       DECIMAL(18,0)     NOT NULL,
    Description NVARCHAR(MAX)     NULL,
    MainImage   VARCHAR(255)      NULL,
    Gender      VARCHAR(10)       NULL,        -- Boy / Girl / Unisex
    CreatedDate DATETIME          NOT NULL CONSTRAINT DF_Products_Created  DEFAULT (GETDATE()),
    IsActive    BIT               NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT (1),
    CONSTRAINT PK_Products          PRIMARY KEY (ProductId),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories (CategoryId),
    CONSTRAINT CK_Products_Price    CHECK (Price >= 0),
    CONSTRAINT CK_Products_Gender   CHECK (Gender IS NULL OR Gender IN ('Boy', 'Girl', 'Unisex'))
);
GO

-- 3.4. ProductVariants: biến thể size - màu - tồn kho
CREATE TABLE dbo.ProductVariants
(
    VariantId     INT IDENTITY(1,1) NOT NULL,
    ProductId     INT               NOT NULL,
    Size          VARCHAR(10)       NOT NULL,
    Color         NVARCHAR(30)      NOT NULL,
    StockQuantity INT               NOT NULL CONSTRAINT DF_Variants_Stock DEFAULT (0),
    CONSTRAINT PK_ProductVariants       PRIMARY KEY (VariantId),
    CONSTRAINT FK_Variants_Products     FOREIGN KEY (ProductId) REFERENCES dbo.Products (ProductId),
    CONSTRAINT UQ_Variants_ProductSizeColor UNIQUE (ProductId, Size, Color),
    CONSTRAINT CK_Variants_Stock        CHECK (StockQuantity >= 0)
);
GO

-- 3.5. ProductImages: ảnh chính/phụ
CREATE TABLE dbo.ProductImages
(
    ImageId   INT IDENTITY(1,1) NOT NULL,
    ProductId INT               NOT NULL,
    ImageUrl  VARCHAR(255)      NOT NULL,
    SortOrder INT               NOT NULL CONSTRAINT DF_Images_SortOrder DEFAULT (0),
    CONSTRAINT PK_ProductImages      PRIMARY KEY (ImageId),
    CONSTRAINT FK_Images_Products    FOREIGN KEY (ProductId) REFERENCES dbo.Products (ProductId)
);
GO

-- 3.6. Orders: đơn hàng
CREATE TABLE dbo.Orders
(
    OrderId         INT IDENTITY(1,1) NOT NULL,
    UserId          INT               NOT NULL,
    OrderDate       DATETIME          NOT NULL CONSTRAINT DF_Orders_Date DEFAULT (GETDATE()),
    ReceiverName    NVARCHAR(100)     NOT NULL,
    Phone           VARCHAR(20)       NOT NULL,
    ShippingAddress NVARCHAR(255)     NOT NULL,
    TotalAmount     DECIMAL(18,0)     NOT NULL CONSTRAINT DF_Orders_Total DEFAULT (0),
    Status          VARCHAR(20)       NOT NULL CONSTRAINT DF_Orders_Status DEFAULT ('Pending'),
    Note            NVARCHAR(255)     NULL,
    CONSTRAINT PK_Orders          PRIMARY KEY (OrderId),
    CONSTRAINT FK_Orders_Users    FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId),
    CONSTRAINT CK_Orders_Status   CHECK (Status IN ('Pending', 'Confirmed', 'Shipping', 'Completed', 'Cancelled')),
    CONSTRAINT CK_Orders_Total    CHECK (TotalAmount >= 0)
);
GO

-- 3.7. OrderDetails: chi tiết đơn hàng
CREATE TABLE dbo.OrderDetails
(
    OrderDetailId INT IDENTITY(1,1) NOT NULL,
    OrderId       INT               NOT NULL,
    VariantId     INT               NOT NULL,
    Quantity      INT               NOT NULL,
    UnitPrice     DECIMAL(18,0)     NOT NULL,
    CONSTRAINT PK_OrderDetails        PRIMARY KEY (OrderDetailId),
    CONSTRAINT FK_Details_Orders      FOREIGN KEY (OrderId)   REFERENCES dbo.Orders (OrderId),
    CONSTRAINT FK_Details_Variants    FOREIGN KEY (VariantId) REFERENCES dbo.ProductVariants (VariantId),
    CONSTRAINT CK_Details_Quantity    CHECK (Quantity > 0),
    CONSTRAINT CK_Details_Price       CHECK (UnitPrice >= 0)
);
GO

-- 3.8. Promotions: khuyến mãi
CREATE TABLE dbo.Promotions
(
    PromotionId     INT IDENTITY(1,1) NOT NULL,
    PromotionName   NVARCHAR(150)     NOT NULL,
    DiscountPercent INT               NOT NULL,
    StartDate       DATE              NOT NULL,
    EndDate         DATE              NOT NULL,
    IsActive        BIT               NOT NULL CONSTRAINT DF_Promotions_IsActive DEFAULT (1),
    CONSTRAINT PK_Promotions         PRIMARY KEY (PromotionId),
    CONSTRAINT CK_Promotions_Percent CHECK (DiscountPercent BETWEEN 0 AND 100),
    CONSTRAINT CK_Promotions_Date    CHECK (EndDate >= StartDate)
);
GO

-- 3.9. ProductPromotions: bảng nối N-N
CREATE TABLE dbo.ProductPromotions
(
    ProductId   INT NOT NULL,
    PromotionId INT NOT NULL,
    CONSTRAINT PK_ProductPromotions       PRIMARY KEY (ProductId, PromotionId),
    CONSTRAINT FK_PP_Products             FOREIGN KEY (ProductId)   REFERENCES dbo.Products (ProductId),
    CONSTRAINT FK_PP_Promotions           FOREIGN KEY (PromotionId) REFERENCES dbo.Promotions (PromotionId)
);
GO

/* ---------------------------------------------------------------------
   4. CHỈ MỤC
   --------------------------------------------------------------------- */
CREATE INDEX IX_Products_CategoryId        ON dbo.Products (CategoryId);
CREATE INDEX IX_ProductVariants_ProductId  ON dbo.ProductVariants (ProductId);
CREATE INDEX IX_ProductImages_ProductId    ON dbo.ProductImages (ProductId);
CREATE INDEX IX_Orders_UserId              ON dbo.Orders (UserId);
CREATE INDEX IX_Orders_Status_OrderDate    ON dbo.Orders (Status, OrderDate);
CREATE INDEX IX_OrderDetails_OrderId       ON dbo.OrderDetails (OrderId);
CREATE INDEX IX_ProductPromotions_Promo    ON dbo.ProductPromotions (PromotionId);
GO

/* =====================================================================
   5. DỮ LIỆU MẪU (SEED)
   ===================================================================== */

/* 5.1. Users --------------------------------------------------------
   Mật khẩu lưu SHA-256 hex chữ hoa (khớp C# BitConverter.ToString(...).Replace("-","")):
     admin      -> admin123
     khachhang1 -> 123456
     khachhang2 -> 123456
   Chỉ dùng mật khẩu ASCII để SQL và C# băm ra cùng kết quả.          */
INSERT INTO dbo.Users (Username, PasswordHash, FullName, Email, Phone, Address, Role, IsActive)
VALUES
( N'admin',      CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'admin123'), 2), N'Quản trị viên',   'admin@kidsfashion.vn',   '0900000001', N'TP. Hồ Chí Minh', 'Admin',    1),
( N'khachhang1', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'),   2), N'Nguyễn Minh Anh', 'khach1@example.com',     '0900000002', N'Quận 1, TP. Hồ Chí Minh', 'Customer', 1),
( N'khachhang2', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'),   2), N'Trần Bảo Long',   'khach2@example.com',     '0900000003', N'Thủ Đức, TP. Hồ Chí Minh', 'Customer', 1);
GO

/* 5.2. Categories: 2 cha + 6 con ------------------------------------ */
SET IDENTITY_INSERT dbo.Categories ON;
INSERT INTO dbo.Categories (CategoryId, CategoryName, ParentId, Description) VALUES
(1, N'Thời trang bé trai', NULL, N'Quần áo dành cho bé trai'),
(2, N'Thời trang bé gái',  NULL, N'Quần áo dành cho bé gái');
SET IDENTITY_INSERT dbo.Categories OFF;
GO

INSERT INTO dbo.Categories (CategoryName, ParentId, Description) VALUES
( N'Áo thun bé trai', 1, N'Áo thun, áo polo cho bé trai'),
( N'Quần bé trai',    1, N'Quần short, jogger, kaki cho bé trai'),
( N'Đồ bộ bé trai',   1, N'Set đồ bộ cho bé trai'),
( N'Váy đầm bé gái',  2, N'Váy, đầm cho bé gái'),
( N'Áo bé gái',       2, N'Áo thun, sơ mi, áo khoác cho bé gái'),
( N'Đồ bộ bé gái',    2, N'Set đồ bộ, legging cho bé gái');
GO

/* 5.3. Products: 18 sản phẩm ---------------------------------------- */
INSERT INTO dbo.Products (ProductName, CategoryId, Price, Description, MainImage, Gender, CreatedDate, IsActive)
VALUES
( N'Áo thun bé trai in gấu',      3, 129000, N'Áo thun cotton mềm mại, in hình gấu ngộ nghĩnh, thấm hút mồ hôi tốt.',      '~/Content/images/products/ao-thun-be-trai-in-gau-01.jpg',     'Boy',  '2026-09-01', 1),
( N'Áo thun bé trai tay dài',     3, 149000, N'Áo thun tay dài cotton co giãn, phù hợp mặc thu đông.',                     '~/Content/images/products/ao-thun-be-trai-tay-dai-01.jpg',    'Boy',  '2026-09-02', 1),
( N'Áo polo bé trai',             3, 179000, N'Áo polo cổ bẻ lịch sự, chất liệu cá sấu thoáng mát.',                       '~/Content/images/products/ao-polo-be-trai-01.jpg',            'Boy',  '2026-09-03', 1),
( N'Quần short bé trai',          4,  99000, N'Quần short thun lưng chun, thoải mái vận động.',                            '~/Content/images/products/quan-short-be-trai-01.jpg',         'Boy',  '2026-09-04', 1),
( N'Quần jogger bé trai',         4, 159000, N'Quần jogger bo gấu, co giãn 4 chiều.',                                      '~/Content/images/products/quan-jogger-be-trai-01.jpg',        'Boy',  '2026-09-05', 1),
( N'Quần kaki bé trai',           4, 189000, N'Quần kaki dài, túi hộp tiện dụng, dễ phối đồ.',                             '~/Content/images/products/quan-kaki-be-trai-01.jpg',          'Boy',  '2026-09-06', 1),
( N'Đồ bộ bé trai sọc',           5, 199000, N'Set áo sọc + quần short đồng bộ, chất cotton mát.',                         '~/Content/images/products/do-bo-be-trai-soc-01.jpg',          'Boy',  '2026-09-07', 1),
( N'Đồ bộ bé trai ngắn tay',      5, 169000, N'Set đồ bộ ngắn tay in hình xe hơi.',                                        '~/Content/images/products/do-bo-be-trai-ngan-tay-01.jpg',     'Boy',  '2026-09-08', 1),
( N'Váy công chúa bé gái',        6, 289000, N'Váy xòe nhiều tầng voan, phù hợp dự tiệc.',                                 '~/Content/images/products/vay-cong-chua-be-gai-01.jpg',       'Girl', '2026-09-09', 1),
( N'Váy hoa nhí bé gái',          6, 259000, N'Váy hoa nhí tay bồng, chất voan mềm.',                                      '~/Content/images/products/vay-hoa-nhi-be-gai-01.jpg',         'Girl', '2026-09-10', 1),
( N'Đầm dâu tây bé gái',          6, 239000, N'Đầm in dâu tây đáng yêu, có túi hai bên.',                                  '~/Content/images/products/dam-dau-tay-be-gai-01.jpg',         'Girl', '2026-09-11', 1),
( N'Áo thun bé gái in hoa',       7, 129000, N'Áo thun cotton in hoa nhí, màu pastel dễ thương.',                          '~/Content/images/products/ao-thun-be-gai-in-hoa-01.jpg',      'Girl', '2026-09-12', 1),
( N'Áo sơ mi bé gái',             7, 169000, N'Áo sơ mi tay dài sọc nhỏ, chất linen thoáng.',                              '~/Content/images/products/ao-so-mi-be-gai-01.jpg',            'Girl', '2026-09-13', 1),
( N'Áo khoác len bé gái',         7, 219000, N'Áo khoác len dệt kim giữ ấm, phối cùng váy rất xinh.',                      '~/Content/images/products/ao-khoac-len-be-gai-01.jpg',        'Girl', '2026-09-14', 1),
( N'Đồ bộ bé gái hoạt hình',      8, 189000, N'Set đồ bộ in hình hoạt hình, thun cotton mềm.',                             '~/Content/images/products/do-bo-be-gai-hoat-hinh-01.jpg',     'Girl', '2026-09-15', 1),
( N'Đồ bộ bé gái mùa hè',         8, 179000, N'Set áo ba lỗ + quần short mùa hè.',                                         '~/Content/images/products/do-bo-be-gai-mua-he-01.jpg',        'Girl', '2026-09-16', 1),
( N'Quần legging bé gái',         8, 119000, N'Quần legging co giãn, mặc nhà hoặc phối váy.',                              '~/Content/images/products/quan-legging-be-gai-01.jpg',        'Girl', '2026-09-17', 1),
( N'Váy bé gái dự tiệc',          6, 329000, N'Váy dự tiệc sang trọng, đính nơ và cườm.',                                  '~/Content/images/products/vay-be-gai-du-tiec-01.jpg',         'Girl', '2026-09-18', 1);
GO

/* 5.4. ProductVariants: mỗi sản phẩm 3 variant -----------------------
   VariantId sinh tuần tự 1..54 theo thứ tự bên dưới.                */
INSERT INTO dbo.ProductVariants (ProductId, Size, Color, StockQuantity)
SELECT p.ProductId, v.Size, v.Color, v.StockQuantity
FROM (VALUES
    (N'Áo thun bé trai in gấu',   '1-2T', N'Trắng',    20),
    (N'Áo thun bé trai in gấu',   '3-4T', N'Trắng',    15),
    (N'Áo thun bé trai in gấu',   '5-6T', N'Xanh navy', 12),
    (N'Áo thun bé trai tay dài',  '1-2T', N'Xám',      15),
    (N'Áo thun bé trai tay dài',  '3-4T', N'Xám',      10),
    (N'Áo thun bé trai tay dài',  '5-6T', N'Xanh rêu',  0),  -- hết hàng: test tồn kho
    (N'Áo polo bé trai',          '1-2T', N'Trắng',    12),
    (N'Áo polo bé trai',          '3-4T', N'Xanh navy', 14),
    (N'Áo polo bé trai',          '5-6T', N'Đỏ',        9),
    (N'Quần short bé trai',       '1-2T', N'Xám',      25),
    (N'Quần short bé trai',       '3-4T', N'Xám',      18),
    (N'Quần short bé trai',       '5-6T', N'Đen',      11),
    (N'Quần jogger bé trai',      '1-2T', N'Xanh navy', 16),
    (N'Quần jogger bé trai',      '3-4T', N'Xanh navy', 13),
    (N'Quần jogger bé trai',      '5-6T', N'Đen',      10),
    (N'Quần kaki bé trai',        '1-2T', N'Be',       14),
    (N'Quần kaki bé trai',        '3-4T', N'Be',       12),
    (N'Quần kaki bé trai',        '5-6T', N'Nâu',       8),
    (N'Đồ bộ bé trai sọc',        '1-2T', N'Xanh trắng', 10),
    (N'Đồ bộ bé trai sọc',        '3-4T', N'Xanh trắng', 14),
    (N'Đồ bộ bé trai sọc',        '5-6T', N'Đỏ trắng',    7),
    (N'Đồ bộ bé trai ngắn tay',   '1-2T', N'Vàng',      12),
    (N'Đồ bộ bé trai ngắn tay',   '3-4T', N'Vàng',      15),
    (N'Đồ bộ bé trai ngắn tay',   '5-6T', N'Xanh',      10),
    (N'Váy công chúa bé gái',     '1-2T', N'Hồng',       8),
    (N'Váy công chúa bé gái',     '3-4T', N'Hồng',      12),
    (N'Váy công chúa bé gái',     '5-6T', N'Tím',        6),
    (N'Váy hoa nhí bé gái',       '1-2T', N'Vàng hoa',  10),
    (N'Váy hoa nhí bé gái',       '3-4T', N'Hồng hoa',  14),
    (N'Váy hoa nhí bé gái',       '5-6T', N'Xanh hoa',   9),
    (N'Đầm dâu tây bé gái',       '1-2T', N'Đỏ',         6),
    (N'Đầm dâu tây bé gái',       '3-4T', N'Đỏ',        11),
    (N'Đầm dâu tây bé gái',       '5-6T', N'Đỏ',         5),
    (N'Áo thun bé gái in hoa',    '1-2T', N'Hồng nhạt', 18),
    (N'Áo thun bé gái in hoa',    '3-4T', N'Hồng nhạt', 16),
    (N'Áo thun bé gái in hoa',    '5-6T', N'Trắng',     13),
    (N'Áo sơ mi bé gái',          '1-2T', N'Trắng sọc', 12),
    (N'Áo sơ mi bé gái',          '3-4T', N'Trắng sọc', 10),
    (N'Áo sơ mi bé gái',          '5-6T', N'Xanh sọc',   8),
    (N'Áo khoác len bé gái',      '1-2T', N'Be',         7),
    (N'Áo khoác len bé gái',      '3-4T', N'Be',        10),
    (N'Áo khoác len bé gái',      '5-6T', N'Ghi',        5),
    (N'Đồ bộ bé gái hoạt hình',   '1-2T', N'Tím',       10),
    (N'Đồ bộ bé gái hoạt hình',   '3-4T', N'Tím',       14),
    (N'Đồ bộ bé gái hoạt hình',   '5-6T', N'Hồng',      12),
    (N'Đồ bộ bé gái mùa hè',      '1-2T', N'Xanh mint', 15),
    (N'Đồ bộ bé gái mùa hè',      '3-4T', N'Xanh mint', 13),
    (N'Đồ bộ bé gái mùa hè',      '5-6T', N'Trắng',     11),
    (N'Quần legging bé gái',      '1-2T', N'Đen',       20),
    (N'Quần legging bé gái',      '3-4T', N'Đen',       17),
    (N'Quần legging bé gái',      '5-6T', N'Hồng',      12),
    (N'Váy bé gái dự tiệc',       '1-2T', N'Trắng',      5),
    (N'Váy bé gái dự tiệc',       '3-4T', N'Trắng',      8),
    (N'Váy bé gái dự tiệc',       '5-6T', N'Xanh đen',   4)
) AS v(ProductName, Size, Color, StockQuantity)
JOIN dbo.Products p ON p.ProductName = v.ProductName;
GO

/* 5.5. ProductImages: 2 ảnh mỗi sản phẩm (01: chính, 02: phụ) -------- */
INSERT INTO dbo.ProductImages (ProductId, ImageUrl, SortOrder)
SELECT p.ProductId,
       '~/Content/images/products/' + v.Slug + CASE WHEN n.SortOrder = 1 THEN '-01.jpg' ELSE '-02.jpg' END,
       n.SortOrder
FROM (VALUES
    (N'Áo thun bé trai in gấu',   'ao-thun-be-trai-in-gau'),
    (N'Áo thun bé trai tay dài',  'ao-thun-be-trai-tay-dai'),
    (N'Áo polo bé trai',          'ao-polo-be-trai'),
    (N'Quần short bé trai',       'quan-short-be-trai'),
    (N'Quần jogger bé trai',      'quan-jogger-be-trai'),
    (N'Quần kaki bé trai',        'quan-kaki-be-trai'),
    (N'Đồ bộ bé trai sọc',        'do-bo-be-trai-soc'),
    (N'Đồ bộ bé trai ngắn tay',   'do-bo-be-trai-ngan-tay'),
    (N'Váy công chúa bé gái',     'vay-cong-chua-be-gai'),
    (N'Váy hoa nhí bé gái',       'vay-hoa-nhi-be-gai'),
    (N'Đầm dâu tây bé gái',       'dam-dau-tay-be-gai'),
    (N'Áo thun bé gái in hoa',    'ao-thun-be-gai-in-hoa'),
    (N'Áo sơ mi bé gái',          'ao-so-mi-be-gai'),
    (N'Áo khoác len bé gái',      'ao-khoac-len-be-gai'),
    (N'Đồ bộ bé gái hoạt hình',   'do-bo-be-gai-hoat-hinh'),
    (N'Đồ bộ bé gái mùa hè',      'do-bo-be-gai-mua-he'),
    (N'Quần legging bé gái',      'quan-legging-be-gai'),
    (N'Váy bé gái dự tiệc',       'vay-be-gai-du-tiec')
) AS v(ProductName, Slug)
JOIN dbo.Products p ON p.ProductName = v.ProductName
CROSS JOIN (VALUES (1), (2)) AS n(SortOrder);
GO

/* 5.6. Promotions: 2 đang hiệu lực + 1 sắp chạy --------------------- */
INSERT INTO dbo.Promotions (PromotionName, DiscountPercent, StartDate, EndDate, IsActive)
VALUES
( N'Khuyến mãi hè 2026',      10, '2026-06-01', '2026-12-31', 1),
( N'Mừng năm học mới 2026',   15, '2026-08-01', '2026-11-30', 1),
( N'Flash sale cuối tuần',    20, '2026-12-01', '2026-12-31', 1);
GO

INSERT INTO dbo.ProductPromotions (ProductId, PromotionId)
SELECT p.ProductId, m.PromotionId
FROM (VALUES
    (N'Áo thun bé trai in gấu',   1),
    (N'Quần short bé trai',       1),
    (N'Áo thun bé gái in hoa',    1),
    (N'Đồ bộ bé gái hoạt hình',   1),
    (N'Quần jogger bé trai',      2),
    (N'Quần kaki bé trai',        2),
    (N'Đồ bộ bé trai sọc',        2),
    (N'Váy hoa nhí bé gái',       2),
    (N'Áo sơ mi bé gái',          2),
    (N'Váy công chúa bé gái',     3),
    (N'Váy bé gái dự tiệc',       3)
) AS m(ProductName, PromotionId)
JOIN dbo.Products p ON p.ProductName = m.ProductName;
GO

/* 5.7. Orders: 5 đơn đủ trạng thái ----------------------------------
   OrderId sinh tuần tự 1..5 theo thứ tự bên dưới.                    */
INSERT INTO dbo.Orders (UserId, OrderDate, ReceiverName, Phone, ShippingAddress, TotalAmount, Status, Note)
VALUES
(2, '2026-09-20 09:30:00', N'Nguyễn Minh Anh', '0900000002', N'12 Lê Lợi, Quận 1, TP. Hồ Chí Minh',  0, 'Completed', N'Giao nhanh giúp em'),
(3, '2026-09-25 14:15:00', N'Trần Bảo Long',   '0900000003', N'45 Võ Văn Ngân, Thủ Đức, TP. Hồ Chí Minh', 0, 'Completed', NULL),
(2, '2026-10-05 10:00:00', N'Nguyễn Minh Anh', '0900000002', N'12 Lê Lợi, Quận 1, TP. Hồ Chí Minh',  0, 'Shipping',  N'Gọi trước khi giao'),
(3, '2026-10-12 16:45:00', N'Trần Bảo Long',   '0900000003', N'45 Võ Văn Ngân, Thủ Đức, TP. Hồ Chí Minh', 0, 'Pending',   NULL),
(2, '2026-10-15 08:20:00', N'Nguyễn Minh Anh', '0900000002', N'12 Lê Lợi, Quận 1, TP. Hồ Chí Minh',  0, 'Cancelled', N'Khách đổi ý');
GO

INSERT INTO dbo.OrderDetails (OrderId, VariantId, Quantity, UnitPrice)
SELECT d.OrderId, v.VariantId, d.Quantity, d.UnitPrice
FROM (VALUES
    (1, N'Áo thun bé trai in gấu', '1-2T', 2, 129000),
    (1, N'Quần short bé trai',     '3-4T', 1,  99000),
    (2, N'Váy công chúa bé gái',   '3-4T', 1, 289000),
    (2, N'Áo thun bé gái in hoa',  '1-2T', 1, 129000),
    (3, N'Đồ bộ bé trai sọc',      '3-4T', 1, 199000),
    (3, N'Quần kaki bé trai',      '1-2T', 1, 189000),
    (3, N'Quần jogger bé trai',    '1-2T', 1, 159000),
    (4, N'Váy bé gái dự tiệc',     '1-2T', 1, 329000),
    (4, N'Đồ bộ bé gái mùa hè',    '1-2T', 2, 179000),
    (5, N'Áo thun bé trai tay dài','1-2T', 1, 149000)
) AS d(OrderId, ProductName, Size, Quantity, UnitPrice)
JOIN dbo.Products p        ON p.ProductName = d.ProductName
JOIN dbo.ProductVariants v ON v.ProductId = p.ProductId AND v.Size = d.Size;

-- Đồng bộ TotalAmount của mỗi đơn theo tổng chi tiết (nhất quán dữ liệu)
UPDATE o
SET o.TotalAmount = x.Total
FROM dbo.Orders o
JOIN (SELECT OrderId, SUM(Quantity * UnitPrice) AS Total
      FROM dbo.OrderDetails GROUP BY OrderId) x ON x.OrderId = o.OrderId;
GO

/* ---------------------------------------------------------------------
   6. KIỂM TRA NHANH SAU KHI CHẠY
   ---------------------------------------------------------------------
   SELECT COUNT(*) AS Users    FROM dbo.Users;            -- 3
   SELECT COUNT(*) AS Cats     FROM dbo.Categories;       -- 8
   SELECT COUNT(*) AS Products FROM dbo.Products;         -- 18
   SELECT COUNT(*) AS Variants FROM dbo.ProductVariants;  -- 54
   SELECT COUNT(*) AS Images   FROM dbo.ProductImages;    -- 36
   SELECT COUNT(*) AS Orders   FROM dbo.Orders;           -- 5
   SELECT SUM(TotalAmount) AS Revenue FROM dbo.Orders WHERE Status = 'Completed'; -- 775000
   --------------------------------------------------------------------- */
