using System;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class TimeValuePoint : EntityBase
    {
        private decimal y;
        private DateTime x;

        public decimal Y
        {
            get { return y; }
            set
            {
                if (y != value)
                {
                    y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }

        public DateTime X
        {
            get { return x; }
            set
            {
                if (x != value)
                {
                    x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }
    }
}
