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
    }
}
