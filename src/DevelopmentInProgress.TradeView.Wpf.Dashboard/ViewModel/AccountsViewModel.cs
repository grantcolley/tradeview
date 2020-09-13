using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel
{
    public class AccountsViewModel : DocumentViewModel
    {
        private readonly IAccountsService accountsService;
        private bool isLoading;
        private bool disposed;

        public AccountsViewModel(ViewModelContext viewModelContext, IAccountsService accountsService)
            : base(viewModelContext)
        {
            this.accountsService = accountsService;

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
        protected async override void OnPublished(object data)
        {
            try
            {
                IsLoading = true;

                ClearAccounts();

                var accounts = await accountsService.GetAccountsAsync().ConfigureAwait(true);

                //accounts.Accounts.ForEach(Accounts.Add);
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
