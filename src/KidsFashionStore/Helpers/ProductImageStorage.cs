using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace KidsFashionStore.Helpers
{
    /// <summary>
    /// Upload ảnh sản phẩm vào ~/Content/images/products/ (đường dẫn tương đối, chạy được trên máy khác).
    /// DB lưu dạng "~/Content/images/products/ten-san-pham-ab12cd34.jpg" giống dữ liệu seed.
    /// </summary>
    public static class ProductImageStorage
    {
        public const string Folder = "~/Content/images/products/";
        public const int MaxBytes = 2 * 1024 * 1024; // 2 MB
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        /// <summary>Trả về thông báo lỗi, hoặc null nếu file hợp lệ.</summary>
        public static string Validate(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0) return "Vui lòng chọn file ảnh.";
            if (file.ContentLength > MaxBytes) return "Ảnh quá lớn (tối đa 2 MB).";

            var ext = (Path.GetExtension(file.FileName ?? "") ?? "").ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext)) return "Chỉ chấp nhận ảnh .jpg, .jpeg, .png, .webp.";
            if (!HasImageSignature(file)) return "File không phải ảnh hợp lệ.";
            return null;
        }

        public static string Save(HttpPostedFileBase file, HttpServerUtilityBase server, string baseName)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = Slugify(baseName) + "-" + Guid.NewGuid().ToString("N").Substring(0, 8) + ext;
            var physicalDir = server.MapPath(Folder);
            Directory.CreateDirectory(physicalDir);
            file.SaveAs(Path.Combine(physicalDir, fileName));
            return Folder + fileName;
        }

        public static string Slugify(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "product";
            var normalized = text.Trim().ToLowerInvariant().Replace('đ', 'd').Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark) continue;
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) sb.Append(c);
                else if (sb.Length > 0 && sb[sb.Length - 1] != '-') sb.Append('-');
            }
            var slug = sb.ToString().Trim('-');
            if (slug.Length > 60) slug = slug.Substring(0, 60).Trim('-');
            return slug.Length == 0 ? "product" : slug;
        }

        // Kiểm tra "magic bytes" để chặn file đổi đuôi (vd: .exe -> .jpg).
        private static bool HasImageSignature(HttpPostedFileBase file)
        {
            var header = new byte[12];
            var stream = file.InputStream;
            stream.Position = 0;
            var read = stream.Read(header, 0, header.Length);
            stream.Position = 0;
            if (read < 12) return false;

            bool jpeg = header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
            bool png = header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47;
            bool webp = header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46
                        && header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;
            return jpeg || png || webp;
        }
    }
}
