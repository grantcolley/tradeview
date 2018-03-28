namespace DevelopmentInProgress.Wpf.Controls.DecimalBox
{
    public static class DecimalExtension
    {
        public static decimal Increment(this decimal value)
        {
            int[] bits1 = decimal.GetBits(value);
            int saved = bits1[3];
            bits1[3] = 0;   // Set scaling to 0, remove sign
            int[] bits2 = decimal.GetBits(new decimal(bits1) + 1);
            bits2[3] = saved; // Restore original scaling and sign
            return new decimal(bits2);
        }

        public static decimal Decrement(this decimal value)
        {
            int[] bits1 = decimal.GetBits(value);
            int saved = bits1[3];
            bits1[3] = 0;   // Set scaling to 0, remove sign
            int[] bits2 = decimal.GetBits(new decimal(bits1) - 1);
            bits2[3] = saved; // Restore original scaling and sign
            return new decimal(bits2);
        }
    }
}
