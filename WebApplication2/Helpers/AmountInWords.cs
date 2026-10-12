using System;
using System.Collections.Generic;

namespace WebApplication2.Helpers
{
    // Indian numbering system: Crore / Lakh / Thousand
    public static class AmountInWords
    {
        private static readonly string[] Ones =
        { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven",
          "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static readonly string[] Tens =
        { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        private static string Below100(int n) => n < 20 ? Ones[n] : (Tens[n / 10] + (n % 10 > 0 ? " " + Ones[n % 10] : ""));

        private static string Below1000(int n)
        {
            var s = "";
            if (n >= 100) { s = Ones[n / 100] + " Hundred"; n %= 100; if (n > 0) s += " "; }
            return s + Below100(n);
        }

        private static string Words(long n)
        {
            if (n == 0) return "Zero";
            var parts = new List<string>();
            long crore = n / 10000000; n %= 10000000;
            long lakh = n / 100000; n %= 100000;
            long thousand = n / 1000; n %= 1000;
            if (crore > 0) parts.Add(Words(crore) + " Crore");
            if (lakh > 0) parts.Add(Below100((int)lakh) + " Lakh");
            if (thousand > 0) parts.Add(Below100((int)thousand) + " Thousand");
            if (n > 0) parts.Add(Below1000((int)n));
            return string.Join(" ", parts);
        }

        // 8968 -> "Rupees Eight Thousand Nine Hundred Sixty Eight Only."
        public static string Convert(decimal amount)
        {
            long rupees = (long)Math.Floor(amount);
            int paise = (int)Math.Round((amount - rupees) * 100);
            if (paise == 100) { rupees++; paise = 0; }
            var s = "Rupees " + Words(rupees);
            if (paise > 0) s += " and " + Words(paise) + " Paise";
            return s + " Only.";
        }
    }
}
