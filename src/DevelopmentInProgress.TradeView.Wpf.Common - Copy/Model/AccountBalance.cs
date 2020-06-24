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
                    OnPropertyChanged("Asset");
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
                    OnPropertyChanged("Free");
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
                    OnPropertyChanged("Locked");
                }
            }
        }
    }
}
