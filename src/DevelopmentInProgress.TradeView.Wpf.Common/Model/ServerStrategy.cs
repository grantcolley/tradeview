using System;
using System.Collections.ObjectModel;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class ServerStrategy : EntityBase
    {
        private string name;
        private string startedBy;
        private string stoppedBy;
        private DateTime started;
        private DateTime stopped;
        private string parameters;

        public ServerStrategy()
        {
            Connections = new ObservableCollection<Connection>();
        }

        public ObservableCollection<Connection> Connections { get; }

        public string Name 
        {
            get { return name; }
            set
            {
                if(name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string StartedBy
        {
            get { return startedBy; }
            set
            {
                if (startedBy != value)
                {
                    startedBy = value;
                    OnPropertyChanged(nameof(StartedBy));
                }
            }
        }

        public string StoppedBy
        {
            get { return stoppedBy; }
            set
            {
                if (stoppedBy != value)
                {
                    stoppedBy = value;
                    OnPropertyChanged(nameof(StoppedBy));
                }
            }
        }

        public DateTime Started
        {
            get { return started; }
            set
            {
                if (started != value)
                {
                    started = value;
                    OnPropertyChanged(nameof(Started));
                }
            }
        }

        public DateTime Stopped
        {
            get { return stopped; }
            set
            {
                if (stopped != value)
                {
                    stopped = value;
                    OnPropertyChanged(nameof(Stopped));
                }
            }
        }

        public string Parameters
        {
            get { return parameters; }
            set
            {
                if (parameters != value)
                {
                    parameters = value;
                    OnPropertyChanged(nameof(Parameters));
                }
            }
        }

        public int ConnectionCount
        {
            get { return Connections.Count; }
            set { OnPropertyChanged(nameof(ConnectionCount)); }
        }
    }
}
