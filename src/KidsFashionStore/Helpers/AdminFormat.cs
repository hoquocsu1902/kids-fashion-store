using System.Globalization;

namespace KidsFashionStore.Helpers
{
    public static class AdminFormat
    {
        private static readonly CultureInfo VN = new CultureInfo("vi-VN");

        public static string Money(decimal value)
        {
            return value.ToString("N0", VN) + " ₫";
        }

        public static string GenderLabel(string gender)
        {
            switch (gender)
            {
                case "Boy": return "Bé trai";
                case "Girl": return "Bé gái";
                case "Unisex": return "Unisex";
                default: return "-";
            }
        }
    }
}
