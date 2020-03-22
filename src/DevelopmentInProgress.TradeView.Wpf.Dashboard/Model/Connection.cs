using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.Model
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
                    OnPropertyChanged("Name");
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
                    OnPropertyChanged("Connected");
                }
            }
        }
    }
}