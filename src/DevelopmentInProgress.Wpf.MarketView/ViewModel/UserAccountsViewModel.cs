using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class UserAccountsViewModel : DocumentViewModel
    {
        private IAccountsService accountsService;

        public UserAccountsViewModel(ViewModelContext viewModelContext, IAccountsService accountsService)
            : base(viewModelContext)
        {
            this.accountsService = accountsService;

            AddAccountCommand = new ViewModelCommand(AddAccount);
            DeleteAccountCommand = new ViewModelCommand(DeleteAccount);
        }

        public ICommand AddAccountCommand { get; set; }
        public ICommand DeleteAccountCommand { get; set; }

        public ObservableCollection<UserAccount> Accounts { get; set; }

        protected override void OnPublished(object data)
        {
            base.OnPublished(data);

            var accounts = accountsService.GetAccounts();
            Accounts = new ObservableCollection<UserAccount>(accounts.Accounts);
        }

        private void AddAccount(object param)
        {
            if (param == null 
                || string.IsNullOrEmpty(param.ToString()))
            {
                return;
            }

            var userAccount = new UserAccount { AccountName = param.ToString() };
            accountsService.SaveAccount(userAccount);
            Accounts.Add(userAccount);
            Module.AddAccount(userAccount.AccountName);
        }

        private void DeleteAccount(object param)
        {
            var userAccount = param as UserAccount;
            if(userAccount == null)
            {
                return;
            }

            accountsService.DeleteAccount(userAccount);
            Accounts.Remove(userAccount);
            Module.RemoveAccount(userAccount.AccountName);
        }
    }
}
