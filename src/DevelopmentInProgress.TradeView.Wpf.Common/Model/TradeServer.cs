using System;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class TradeServer : EntityBase
    {
        private string name;
        private Uri uri;
        private int maxDegreeOfParallelism;
        private bool enabled;

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

        public Uri Uri
        {
            get { return uri; }
            set
            {
                if (uri != value)
                {
                    uri = value;
                    OnPropertyChanged("Uri");
                }
            }
        }

        public int MaxDegreeOfParallelism
        {
            get { return maxDegreeOfParallelism; }
            set
            {
                if (maxDegreeOfParallelism != value)
                {
                    maxDegreeOfParallelism = value;
                    OnPropertyChanged("MaxDegreeOfParallelism");
                }
            }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnPropertyChanged("Enabled");
                }
            }
        }
    }
}
