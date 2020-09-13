using DevelopmentInProgress.TradeView.Wpf.Common.Cache;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel;
using Prism.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel
{
    public class AccountsViewModel : DocumentViewModel
    {
        private readonly IAccountsService accountsService;
        private readonly IWpfExchangeService exchangeService;
        private readonly ISymbolsCacheFactory symbolsCacheFactory;
        private readonly ILoggerFacade logger;
        private bool isLoading;
        private bool disposed;

        public AccountsViewModel(
            ViewModelContext viewModelContext, 
            IAccountsService accountsService, 
            IWpfExchangeService exchangeService, 
            ISymbolsCacheFactory symbolsCacheFactory, 
            ILoggerFacade logger)
            : base(viewModelContext)
        {
            this.accountsService = accountsService;
            this.exchangeService = exchangeService;
            this.symbolsCacheFactory = symbolsCacheFactory;
            this.logger = logger;

            Accounts = new ObservableCollection<AccountViewModel>();

            IsLoading = true;
        }

        public ObservableCollection<AccountViewModel> Accounts { get; }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Exceptions are routed back to subscribers.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Objects get disposed in the Dispose method.")]
        protected async override void OnPublished(object data)
        {
            try
            {
                IsLoading = true;

                ClearAccounts();

                var accounts = await accountsService.GetAccountsAsync().ConfigureAwait(true);

                var loginTasks = new Task[accounts.Accounts.Count];

                for (int i = 0; i < accounts.Accounts.Count; i++)
                {
                    var userAccount = accounts.Accounts[i];
                    if (!string.IsNullOrWhiteSpace(userAccount.ApiKey))
                    {
                        var account = new Account(new Core.Model.AccountInfo { User = new Core.Model.User() })
                        {
                            AccountName = userAccount.AccountName,
                            ApiKey = userAccount.ApiKey,
                            ApiSecret = userAccount.ApiSecret,
                            ApiPassPhrase = userAccount.ApiPassPhrase,
                            Exchange = userAccount.Exchange
                        };

                        var accountViewModel = new AccountViewModel(
                            new AccountBalancesViewModel(exchangeService, symbolsCacheFactory, logger),
                            new OrdersViewModel(exchangeService, logger),
                            logger);

                        accountViewModel.SetAccount(account);
                        loginTasks[i] = accountViewModel.Login();
                        Accounts.Add(accountViewModel);
                    }
                }

                await Task.WhenAll(loginTasks).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message, TextVerbose = ex.StackTrace });
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected override void OnDisposing()
        {
            if (disposed)
            {
                return;
            }

            ClearAccounts();

            disposed = true;
        }

        private void ClearAccounts()
        {
            if (Accounts.Any())
            {
                foreach (var accountViewModel in Accounts)
                {
                    accountViewModel.Dispose();
                    Accounts.Remove(accountViewModel);
                }
            }
        }
    }
}
