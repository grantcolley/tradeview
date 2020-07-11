using System;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class Connection : EntityBase
    {
        private string name;
        private DateTime connected;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public DateTime Connected
        {
            get { return connected; }
            set
            {
                if (connected != value)
                {
                    connected = value;
                    OnPropertyChanged(nameof(Connected));
                }
            }
        }
    }
}