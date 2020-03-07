namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class Server : EntityBase
    {
        private string name;
        private string url;

        public string Name 
        {
            get { return name; }
            set
            {
                if(name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Url
        {
            get { return url; }
            set
            {
                if (url != value)
                {
                    url = value;
                    OnPropertyChanged("Url");
                }
            }
        }
    }
}
