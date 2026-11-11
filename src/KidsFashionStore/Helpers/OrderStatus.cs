using System.Collections.Generic;
using System.Linq;

namespace KidsFashionStore.Helpers
{
    /// <summary>Tập trạng thái đơn hàng chuẩn (khớp CHECK constraint CK_Orders_Status) + luật chuyển trạng thái.</summary>
    public static class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Shipping = "Shipping";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";

        public static readonly string[] All = { Pending, Confirmed, Shipping, Completed, Cancelled };

        private static readonly Dictionary<string, string> Labels = new Dictionary<string, string>
        {
            { Pending, "Chờ xác nhận" },
            { Confirmed, "Đã xác nhận" },
            { Shipping, "Đang giao" },
            { Completed, "Hoàn thành" },
            { Cancelled, "Đã hủy" }
        };

        // Completed và Cancelled là trạng thái cuối: không đổi lại (bảo vệ số liệu doanh thu).
        private static readonly Dictionary<string, string[]> Next = new Dictionary<string, string[]>
        {
            { Pending,   new[] { Confirmed, Cancelled } },
            { Confirmed, new[] { Shipping, Cancelled } },
            { Shipping,  new[] { Completed, Cancelled } },
            { Completed, new string[0] },
            { Cancelled, new string[0] }
        };

        public static string Label(string status)
        {
            string label;
            return status != null && Labels.TryGetValue(status, out label) ? label : status;
        }

        public static string[] NextOf(string status)
        {
            string[] next;
            return status != null && Next.TryGetValue(status, out next) ? next : new string[0];
        }

        public static bool CanChange(string from, string to)
        {
            return NextOf(from).Contains(to);
        }

        public static string CssClass(string status)
        {
            return "badge badge-" + (status ?? "none").ToLowerInvariant();
        }
    }
}
