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
        public string parameters;
        private ObservableCollection<Connection> connections;

        public ServerStrategy()
        {
            Connections = new ObservableCollection<Connection>();
        }

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

        public string StartedBy
        {
            get { return startedBy; }
            set
            {
                if (startedBy != value)
                {
                    startedBy = value;
                    OnPropertyChanged("StartedBy");
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
                    OnPropertyChanged("StoppedBy");
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
                    OnPropertyChanged("Started");
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
                    OnPropertyChanged("Stopped");
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
                    OnPropertyChanged("Parameters");
                }
            }
        }

        public ObservableCollection<Connection> Connections
        {
            get { return connections; }
            set
            {
                if (connections != value)
                {
                    connections = value;
                    OnPropertyChanged("Connections");
                }
            }
        }

        public int ConnectionCount
        {
            get { return Connections.Count; }
            set { OnPropertyChanged("ConnectionCount"); }
        }
    }
}
