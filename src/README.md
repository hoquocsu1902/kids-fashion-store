# src/ — Solution ASP.NET MVC 5

Thư mục này chứa solution Visual Studio của dự án. **Solution chưa được tạo** vì cần Visual Studio trên Windows
(macOS/Linux không build được .NET Framework).

## Việc cần làm (Tuần 3 — Sử chính, cả nhóm hỗ trợ)

1. Trên máy Windows, mở Visual Studio → `Create a new project` → **ASP.NET Web Application (.NET Framework)**.
2. Đặt tên project: **KidsFashionStore**, vị trí lưu: thư mục `src/` này.
3. Chọn template **MVC**, tick thêm **Web API**.
4. Cấu trúc mong muốn sau khi tạo:

```
src/
├── KidsFashionStore.sln
└── KidsFashionStore/
    ├── App_Start/     RouteConfig.cs, WebApiConfig.cs, FilterConfig.cs
    ├── Controllers/   HomeController.cs, ProductController.cs, ...
    ├── Models/        QL_ThoiTrangTreEm.edmx, ViewModels/, Cart/CartItem.cs
    ├── Views/         Shared/, Home/, Product/, Account/, Cart/, Order/, Admin...
    ├── Content/       site.css, images/products/
    ├── Scripts/
    ├── Database/      (copy từ /Database ở gốc repo khi cần)
    ├── Web.config
    └── Global.asax
```

5. Tạo EF model (Database First) trỏ tới database `QL_ThoiTrangTreEm`.
6. Commit + push, mở Pull Request vào `develop` để cả nhóm kéo về.

## Lưu ý

- Tên project/solution phải đúng **KidsFashionStore** để thống nhất connection string và namespace.
- Không commit `bin/`, `obj/`, `packages/`, `.vs/` (đã có trong `.gitignore` ở gốc repo).
- Ảnh sản phẩm đặt tại `KidsFashionStore/Content/images/products/` theo naming convention trong
  [`../Database/README.md`](../Database/README.md) mục 5.
- Sau khi có solution, cập nhật lại `README.md` ở gốc repo nếu cấu trúc thay đổi.
