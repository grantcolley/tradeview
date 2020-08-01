using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "General exceptions are written to the log file and shown in the message panel.")]
    public class TradeServerManagerViewModel : DocumentViewModel
    {
        private readonly ITradeServerService tradeServerService;
        private readonly Dictionary<string, IDisposable> tradeServerObservableSubscriptions;
        private TradeServerViewModel selectedTradeServerViewModel;
        private TradeServer selectedTradeServer;
        private bool isLoading;
        private bool disposed;

        public TradeServerManagerViewModel(ViewModelContext viewModelContext, ITradeServerService tradeServerService)
            : base(viewModelContext)
        {
            this.tradeServerService = tradeServerService;

            AddTradeServerCommand = new ViewModelCommand(AddTradeServer);
            DeleteTradeServerCommand = new ViewModelCommand(DeleteTradeServer);
            CloseCommand = new ViewModelCommand(Close);

            SelectedTradeServerViewModels = new ObservableCollection<TradeServerViewModel>();
            tradeServerObservableSubscriptions = new Dictionary<string, IDisposable>();
        }

        public ICommand AddTradeServerCommand { get; set; }
        public ICommand DeleteTradeServerCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public ObservableCollection<TradeServerViewModel> SelectedTradeServerViewModels { get; }
        public ObservableCollection<TradeServer> TradeServers { get; }

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        public TradeServer SelectedTradeServer
        {
            get { return selectedTradeServer; }
            set
            {
                if (selectedTradeServer != value)
                {
                    selectedTradeServer = value;

                    if (selectedTradeServer != null)
                    {
                        var serverViewModel = SelectedTradeServerViewModels.FirstOrDefault(s => s.TradeServer.Name.Equals(selectedTradeServer.Name, StringComparison.Ordinal));

                        if (serverViewModel == null)
                        {
                            serverViewModel = new TradeServerViewModel(selectedTradeServer, Logger);
                            ObserveServer(serverViewModel);
                            SelectedTradeServerViewModels.Add(serverViewModel);
                            SelectedTradeServerViewModel = serverViewModel;
                        }
                        else
                        {
                            SelectedTradeServerViewModel = serverViewModel;
                        }
                    }

                    OnPropertyChanged(nameof(SelectedTradeServer));
                }
            }
        }

        public TradeServerViewModel SelectedTradeServerViewModel
        {
            get { return selectedTradeServerViewModel; }
            set
            {
                if (selectedTradeServerViewModel != value)
                {
                    selectedTradeServerViewModel = value;
                    OnPropertyChanged(nameof(SelectedTradeServerViewModel));
                }
            }
        }

        public void Close(object param)
        {
            if (param is TradeServerViewModel tradeServer)
            {
                tradeServer.Dispose();

                if (tradeServerObservableSubscriptions.TryGetValue(tradeServer.TradeServer.Name, out IDisposable subscription))
                {
                    subscription.Dispose();
                }

                tradeServerObservableSubscriptions.Remove(tradeServer.TradeServer.Name);
                
                SelectedTradeServerViewModels.Remove(tradeServer);
            }
        }

        protected async override void OnPublished(object data)
        {
            base.OnPublished(data);

            try
            {
                IsLoading = true;

                var tradeServers = await tradeServerService.GetTradeServers().ConfigureAwait(true);

                TradeServers.Clear();
                tradeServers.ForEach(TradeServers.Add);
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

            foreach (var subscription in tradeServerObservableSubscriptions.Values)
            {
                subscription.Dispose();
            }

            foreach (var tradeServerViewModel in SelectedTradeServerViewModels)
            {
                tradeServerViewModel.Dispose();
            }

            disposed = true;
        }

        protected async override void SaveDocument()
        {
            try
            {
                IsLoading = true;

                foreach (var serverViewModel in SelectedTradeServerViewModels)
                {
                    await tradeServerService.SaveTradeServer(serverViewModel.TradeServer).ConfigureAwait(false);
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

        private async void AddTradeServer(object param)
        {
            if (param == null
                || string.IsNullOrEmpty(param.ToString()))
            {
                return;
            }

            var tradeServerName = param.ToString();

            if (TradeServers.Any(s => s.Name.Equals(tradeServerName, StringComparison.Ordinal)))
            {
                ShowMessage(new Message { MessageType = MessageType.Info, Text = $"A trade server with the name {tradeServerName} already exists." });
                return;
            }

            try
            {
                IsLoading = true;

                var tradeServer = new TradeServer { Name = tradeServerName };
                await tradeServerService.SaveTradeServer(tradeServer).ConfigureAwait(true);
                TradeServers.Add(tradeServer);
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

        private async void DeleteTradeServer(object param)
        {
            if (param is TradeServer tradeServer)
            {
                var result = Dialog.ShowMessage(new MessageBoxSettings
                {
                    Title = "Delete Trade Server",
                    Text = $"Are you sure you want to delete {tradeServer.Name}?",
                    MessageType = MessageType.Question,
                    MessageBoxButtons = MessageBoxButtons.OkCancel
                });

                if (result.Equals(MessageBoxResult.Cancel))
                {
                    return;
                }

                var tradeServerViewModel = SelectedTradeServerViewModels.FirstOrDefault(s => s.TradeServer.Name.Equals(tradeServer.Name, StringComparison.Ordinal));
                if (tradeServerViewModel != null)
                {
                    Close(tradeServerViewModel);
                }

                try
                {
                    IsLoading = true;

                    await tradeServerService.DeleteTradeServer(tradeServer).ConfigureAwait(true);
                    TradeServers.Remove(tradeServer);
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
        }

        private void ObserveServer(TradeServerViewModel tradeServer)
        {
            var tradeServerObservable = Observable.FromEventPattern<TradeServerEventArgs>(
                eventHandler => tradeServer.OnTradeServerNotification += eventHandler,
                eventHandler => tradeServer.OnTradeServerNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            var tradeServerObservableSubscription = tradeServerObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    ShowMessage(new Message { MessageType = MessageType.Error, Text = args.Exception.ToString() });
                }
            });

            tradeServerObservableSubscriptions.Add(tradeServer.TradeServer.Name, tradeServerObservableSubscription);
        }
    }
}