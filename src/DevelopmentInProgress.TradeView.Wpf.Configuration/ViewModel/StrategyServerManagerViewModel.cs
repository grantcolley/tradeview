using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Reactive.Linq;
using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel
{
    public class StrategyServerManagerViewModel : DocumentViewModel
    {
        private IStrategyServerService strategyServerService;
        private ObservableCollection<StrategyServer> strategyServers;
        private StrategyServerViewModel selectedStrategyServerViewModel;
        private Dictionary<string, IDisposable> strategyServerObservableSubscriptions;
        private StrategyServer selectedStrategyServer;
        private bool isLoading;
        private bool disposed;

        public StrategyServerManagerViewModel(ViewModelContext viewModelContext, IStrategyServerService strategyServerService)
            : base(viewModelContext)
        {
            this.strategyServerService = strategyServerService;

            AddStrategyServerCommand = new ViewModelCommand(AddStrategyServer);
            DeleteStrategyServerCommand = new ViewModelCommand(DeleteStrategyServer);
            CloseCommand = new ViewModelCommand(Close);

            SelectedStrategyServerViewModels = new ObservableCollection<StrategyServerViewModel>();
            strategyServerObservableSubscriptions = new Dictionary<string, IDisposable>();
        }

        public ICommand AddStrategyServerCommand { get; set; }
        public ICommand DeleteStrategyServerCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public ObservableCollection<StrategyServer> StrategyServers 
        {
            get { return strategyServers; } 
            set
            {
                if(strategyServers != value)
                {
                    strategyServers = value;
                    OnPropertyChanged("StrategyServers");
                }
            }
        }

        public ObservableCollection<StrategyServerViewModel> SelectedStrategyServerViewModels { get; set; }

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

        public StrategyServer SelectedStrategyServer
        {
            get { return selectedStrategyServer; }
            set
            {
                if (selectedStrategyServer != value)
                {
                    selectedStrategyServer = value;

                    if (selectedStrategyServer != null)
                    {
                        var strategyServerViewModel = SelectedStrategyServerViewModels.FirstOrDefault(s => s.StrategyServer.Name.Equals(selectedStrategyServer.Name));

                        if (strategyServerViewModel == null)
                        {
                            strategyServerViewModel = new StrategyServerViewModel(selectedStrategyServer, strategyServerService, Logger);
                            ObserveStrategyServer(strategyServerViewModel);
                            SelectedStrategyServerViewModels.Add(strategyServerViewModel);
                            SelectedStrategyServerViewModel = strategyServerViewModel;
                        }
                        else
                        {
                            SelectedStrategyServerViewModel = strategyServerViewModel;
                        }
                    }

                    OnPropertyChanged("SelectedStrategyServer");
                }
            }
        }

        public StrategyServerViewModel SelectedStrategyServerViewModel
        {
            get { return selectedStrategyServerViewModel; }
            set
            {
                if (selectedStrategyServerViewModel != value)
                {
                    selectedStrategyServerViewModel = value;
                    OnPropertyChanged("SelectedStrategyServerViewModel");
                }
            }
        }

        public void Close(object param)
        {
            var strategyServer = param as StrategyServerViewModel;
            if (strategyServer != null)
            {
                strategyServer.Dispose();

                IDisposable subscription;
                if (strategyServerObservableSubscriptions.TryGetValue(strategyServer.StrategyServer.Name, out subscription))
                {
                    subscription.Dispose();
                }

                strategyServerObservableSubscriptions.Remove(strategyServer.StrategyServer.Name);
                
                SelectedStrategyServerViewModels.Remove(strategyServer);
            }
        }

        protected async override void OnPublished(object data)
        {
            base.OnPublished(data);

            try
            {
                IsLoading = true;

                var strategyServers = await strategyServerService.GetStrategyServers();

                StrategyServers = new ObservableCollection<StrategyServer>(strategyServers);
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

            foreach (var subscription in strategyServerObservableSubscriptions.Values)
            {
                subscription.Dispose();
            }

            foreach (var strategyServerViewModel in SelectedStrategyServerViewModels)
            {
                strategyServerViewModel.Dispose();
            }

            disposed = true;
        }

        protected async override void SaveDocument()
        {
            try
            {
                IsLoading = true;

                foreach (var strategyServerViewModel in SelectedStrategyServerViewModels)
                {
                    await strategyServerService.SaveStrategyServer(strategyServerViewModel.StrategyServer);
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

        private async void AddStrategyServer(object param)
        {
            if (param == null
                || string.IsNullOrEmpty(param.ToString()))
            {
                return;
            }

            var strategyServerName = param.ToString();

            if (StrategyServers.Any(s => s.Name.Equals(strategyServerName)))
            {
                ShowMessage(new Message { MessageType = MessageType.Info, Text = $"A strategy server with the name {strategyServerName} already exists." });
                return;
            }

            try
            {
                IsLoading = true;

                var strategyServer = new StrategyServer { Name = strategyServerName };
                await strategyServerService.SaveStrategyServer(strategyServer);
                StrategyServers.Add(strategyServer);
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

        private async void DeleteStrategyServer(object param)
        {
            var strategyServer = param as StrategyServer;
            if (strategyServer == null)
            {
                return;
            }

            var result = Dialog.ShowMessage(new MessageBoxSettings
            {
                Title = "Delete Strategy Server",
                Text = $"Are you sure you want to delete {strategyServer.Name}?",
                MessageType = MessageType.Question,
                MessageBoxButtons = MessageBoxButtons.OkCancel
            });

            if (result.Equals(MessageBoxResult.Cancel))
            {
                return;
            }

            var strategyServerViewModel = SelectedStrategyServerViewModels.FirstOrDefault(s => s.StrategyServer.Name.Equals(strategyServer.Name));
            if(strategyServerViewModel != null)
            {
                Close(strategyServerViewModel);
            }

            try
            {
                IsLoading = true;

                await strategyServerService.DeleteStrategyServer(strategyServer);
                StrategyServers.Remove(strategyServer);
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

        private void ObserveStrategyServer(StrategyServerViewModel strategyServer)
        {
            var strateyServerObservable = Observable.FromEventPattern<StrategyServerEventArgs>(
                eventHandler => strategyServer.OnStrategyServerNotification += eventHandler,
                eventHandler => strategyServer.OnStrategyServerNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            var strateyServerObservableSubscription = strateyServerObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    ShowMessage(new Message { MessageType = MessageType.Error, Text = args.Exception.ToString() });
                }
            });

            strategyServerObservableSubscriptions.Add(strategyServer.StrategyServer.Name, strateyServerObservableSubscription);
        }
    }
}