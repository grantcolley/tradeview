using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel
{
    public class ServerManagerViewModel : DocumentViewModel
    {
        private IServerService serverService;
        private ObservableCollection<Server> servers;
        private ServerViewModel selectedServerViewModel;
        private Dictionary<string, IDisposable> serverObservableSubscriptions;
        private Server selectedServer;
        private bool isLoading;
        private bool disposed;

        public ServerManagerViewModel(ViewModelContext viewModelContext, IServerService serverService)
            : base(viewModelContext)
        {
            this.serverService = serverService;

            AddServerCommand = new ViewModelCommand(AddServer);
            DeleteServerCommand = new ViewModelCommand(DeleteServer);
            CloseCommand = new ViewModelCommand(Close);

            SelectedServerViewModels = new ObservableCollection<ServerViewModel>();
            serverObservableSubscriptions = new Dictionary<string, IDisposable>();
        }

        public ICommand AddServerCommand { get; set; }
        public ICommand DeleteServerCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public ObservableCollection<Server> Servers 
        {
            get { return servers; } 
            set
            {
                if(servers != value)
                {
                    servers = value;
                    OnPropertyChanged("Servers");
                }
            }
        }

        public ObservableCollection<ServerViewModel> SelectedServerViewModels { get; set; }

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged("IsLoading");
                }
            }
        }

        public Server SelectedServer
        {
            get { return selectedServer; }
            set
            {
                if (selectedServer != value)
                {
                    selectedServer = value;

                    if (selectedServer != null)
                    {
                        var serverViewModel = SelectedServerViewModels.FirstOrDefault(s => s.Server.Name.Equals(selectedServer.Name));

                        if (serverViewModel == null)
                        {
                            serverViewModel = new ServerViewModel(selectedServer, serverService, Logger);
                            ObserveServer(serverViewModel);
                            SelectedServerViewModels.Add(serverViewModel);
                            SelectedServerViewModel = serverViewModel;
                        }
                        else
                        {
                            SelectedServerViewModel = serverViewModel;
                        }
                    }

                    OnPropertyChanged("SelectedServer");
                }
            }
        }

        public ServerViewModel SelectedServerViewModel
        {
            get { return selectedServerViewModel; }
            set
            {
                if (selectedServerViewModel != value)
                {
                    selectedServerViewModel = value;
                    OnPropertyChanged("SelectedServerViewModel");
                }
            }
        }

        public void Close(object param)
        {
            var server = param as ServerViewModel;
            if (server != null)
            {
                server.Dispose();

                IDisposable subscription;
                if (serverObservableSubscriptions.TryGetValue(server.Server.Name, out subscription))
                {
                    subscription.Dispose();
                }

                serverObservableSubscriptions.Remove(server.Server.Name);
                
                SelectedServerViewModels.Remove(server);
            }
        }

        protected async override void OnPublished(object data)
        {
            base.OnPublished(data);

            try
            {
                IsLoading = true;

                var servers = await serverService.GetServers();

                Servers = new ObservableCollection<Server>(servers);
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected override void OnDisposing()
        {
            if(disposed)
            {
                return;           
            }

            foreach (var subscription in serverObservableSubscriptions.Values)
            {
                subscription.Dispose();
            }

            foreach (var serverViewModel in SelectedServerViewModels)
            {
                serverViewModel.Dispose();
            }

            disposed = true;
        }

        protected async override void SaveDocument()
        {
            try
            {
                IsLoading = true;

                foreach (var serverViewModel in SelectedServerViewModels)
                {
                    await serverService.SaveServer(serverViewModel.Server);
                }
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void AddServer(object param)
        {
            if (param == null
                || string.IsNullOrEmpty(param.ToString()))
            {
                return;
            }

            var serverName = param.ToString();

            if (Servers.Any(s => s.Name.Equals(serverName)))
            {
                ShowMessage(new Message { MessageType = MessageType.Info, Text = $"A server with the name {serverName} already exists." });
                return;
            }

            try
            {
                IsLoading = true;

                var server = new Server { Name = serverName };
                await serverService.SaveServer(server);
                Servers.Add(server);
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void DeleteServer(object param)
        {
            var server = param as Server;
            if (server == null)
            {
                return;
            }

            var result = Dialog.ShowMessage(new MessageBoxSettings
            {
                Title = "Delete Server",
                Text = $"Are you sure you want to delete {server.Name}?",
                MessageType = MessageType.Question,
                MessageBoxButtons = MessageBoxButtons.OkCancel
            });

            if (result.Equals(MessageBoxResult.Cancel))
            {
                return;
            }

            var serverViewModel = SelectedServerViewModels.FirstOrDefault(s => s.Server.Name.Equals(server.Name));
            if(serverViewModel != null)
            {
                Close(serverViewModel);
            }

            try
            {
                IsLoading = true;

                await serverService.DeleteServer(server);
                Servers.Remove(server);
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ObserveServer(ServerViewModel server)
        {
            var serverObservable = Observable.FromEventPattern<ServerEventArgs>(
                eventHandler => server.OnServerNotification += eventHandler,
                eventHandler => server.OnServerNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            var serverObservableSubscription = serverObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    ShowMessage(new Message { MessageType = MessageType.Error, Text = args.Exception.ToString() });
                }
            });

            serverObservableSubscriptions.Add(server.Server.Name, serverObservableSubscription);
        }
    }
}