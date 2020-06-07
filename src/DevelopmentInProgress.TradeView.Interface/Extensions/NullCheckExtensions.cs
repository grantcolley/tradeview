using System;

namespace DevelopmentInProgress.TradeView.Interface.Extensions
{
    public static class NullCheckExtensions
    {
        public static void NullCheck(this object variable)
        {
            if(variable == null)
            {
                throw new ArgumentNullException(nameof(variable));
            }
        }

        public static void NullCheck(this string variable)
        {
            if (variable == null)
            {
                throw new ArgumentNullException(nameof(variable));
            }
        }
    }
}
