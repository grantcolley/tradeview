namespace DevelopmentInProgress.Wpf.MarketView.Extensions
{
    public static class DecimalHelper
    {
        public static decimal Trim(this decimal v, int precision)
        {
            var p = v.ToString();
            var pos = p.IndexOf(".");

            if(pos == -1)
            {
                return v;
            }

            if(precision > 0)
            {
                pos++;
            }

            return decimal.Parse(p.Substring(0, pos + precision));
        }

        public static bool HasPrecision(this decimal v)
        {
            return (v % 1) == 0;
        }

        public static int GetPrecision(this decimal v)
        {
            var p = v.ToString();
            return p.IndexOf(".") == -1 ? 0 : p.Substring(p.IndexOf(".") + 1).Length;
        }
    }
}
