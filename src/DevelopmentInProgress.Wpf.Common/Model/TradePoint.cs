using System;
using System.ComponentModel;

namespace DevelopmentInProgress.Wpf.Common.Model
{
    public class TradePoint : INotifyPropertyChanged
    {
        private decimal price;
        private DateTime time;

        public event PropertyChangedEventHandler PropertyChanged;

        public decimal Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged("Price");
            }
        }

        public DateTime Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged("Time");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
