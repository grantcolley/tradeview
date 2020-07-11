namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class AccountBalance : EntityBase
    {
        private string asset;
        private decimal free;
        private decimal locked;

        public string Asset
        {
            get { return asset; }
            set
            {
                if (asset != value)
                {
                    asset = value;
                    OnPropertyChanged(nameof(Asset));
                }
            }
        }

        public decimal Free
        {
            get { return free; }
            set
            {
                if (free != value)
                {
                    free = value;
                    OnPropertyChanged(nameof(Free));
                }
            }
        }

        public decimal Locked
        {
            get { return locked; }
            set
            {
                if (locked != value)
                {
                    locked = value;
                    OnPropertyChanged(nameof(Locked));
                }
            }
        }
    }
}
