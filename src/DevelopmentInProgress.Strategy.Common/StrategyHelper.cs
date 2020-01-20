using System;

namespace DevelopmentInProgress.Strategy.Common
{
    public static class StrategyHelper
    {
        public static decimal CalculateMovingAverage(int position, decimal[] decimals, int setLength)
        {
            Span<decimal> slice;
            int sliceCount;

            if (position < setLength)
            {
                slice = new Span<decimal>(decimals, 0, position + 1);
                sliceCount = position + 1;
            }
            else
            {
                slice = new Span<decimal>(decimals, position + 1 - setLength, setLength);
                sliceCount = setLength;
            }

            decimal sumPrice = 0m;

            for (int n = 0; n < sliceCount; n++)
            {
                sumPrice += slice[n];
            }

            var smaPrice = sumPrice / sliceCount;

            return smaPrice;
        }

        public static double CalculateMovingAverage(int position, double[] doubles, int setLength)
        {
            Span<double> slice;
            int sliceCount;

            if (position < setLength)
            {
                slice = new Span<double>(doubles, 0, position + 1);
                sliceCount = position + 1;
            }
            else
            {
                slice = new Span<double>(doubles, position + 1 - setLength, setLength);
                sliceCount = setLength;
            }

            double sumPrice = 0;

            for (int n = 0; n < sliceCount; n++)
            {
                sumPrice += slice[n];
            }

            var smaPrice = sumPrice / sliceCount;

            return smaPrice;
        }

        public static double CalculateMovingVolatility(int position, double[] deviationsSquared, int setLength)
        {
            if (position.Equals(0))
            {
                return 0;
            }

            Span<double> slice;
            int sliceCount;

            if (position < setLength)
            {
                slice = new Span<double>(deviationsSquared, 0, position + 1);
                sliceCount = position + 1;
            }
            else
            {
                slice = new Span<double>(deviationsSquared, position + 1 - setLength, setLength);
                sliceCount = setLength;
            }

            double sumDeviationsSquared = 0;

            for (int n = 0; n < sliceCount; n++)
            {
                sumDeviationsSquared += slice[n];
            }

            var sumDeviationsSquaredMean = sumDeviationsSquared / (sliceCount - 1);

            var volatility = Math.Sqrt(sumDeviationsSquaredMean);

            return volatility;
        }
    }
}
