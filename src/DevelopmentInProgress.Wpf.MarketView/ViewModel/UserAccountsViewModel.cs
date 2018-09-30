using DevelopmentInProgress.Wpf.Controls.Messaging;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class UserAccountsViewModel : DocumentViewModel
    {
        private IAccountsService accountsService;
        private UserAccount selectedUserAccount;
        private UserAccountViewModel selectedUserAccountViewModel;

        public UserAccountsViewModel(ViewModelContext viewModelContext, IAccountsService accountsService)
            : base(viewModelContext)
        {
            this.accountsService = accountsService;

            AddAccountCommand = new ViewModelCommand(AddAccount);
            DeleteAccountCommand = new ViewModelCommand(DeleteAccount);
            CloseCommand = new ViewModelCommand(Close);

            SelectedUserAccountViewModels = new ObservableCollection<UserAccountViewModel>();
        }

        public ICommand AddAccountCommand { get; set; }
        public ICommand DeleteAccountCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public ObservableCollection<UserAccount> Accounts { get; set; }

        public ObservableCollection<UserAccountViewModel> SelectedUserAccountViewModels { get; set; }

        public UserAccount SelectedUserAccount
        {
            get { return selectedUserAccount; }
            set
            {
                if (selectedUserAccount != value)
                {
                    selectedUserAccount = value;

                    if (selectedUserAccount != null)
                    {
                        SelectedUserAccountViewModel = SelectedUserAccountViewModels.FirstOrDefault(s => s.UserAccount.AccountName.Equals(selectedUserAccount.AccountName));

                        if (SelectedUserAccountViewModel == null)
                        {
                            var userAccountViewModel = new UserAccountViewModel(selectedUserAccount);
                            SelectedUserAccountViewModels.Add(userAccountViewModel);
                            SelectedUserAccountViewModel = userAccountViewModel;
                        }
                    }

                    OnPropertyChanged("SelectedUserAccount");
                }
            }
        }

        public UserAccountViewModel SelectedUserAccountViewModel
        {
            get { return selectedUserAccountViewModel; }
            set
            {
                if (selectedUserAccountViewModel != value)
                {
                    selectedUserAccountViewModel = value;
                    OnPropertyChanged("SelectedUserAccountViewModel");
                }
            }
        }

        public void Close(object param)
        {
            var userAccount = param as UserAccountViewModel;
            if (userAccount != null)
            {
                userAccount.Dispose();
                SelectedUserAccountViewModels.Remove(userAccount);
            }
        }

        protected override void OnPublished(object data)
        {
            base.OnPublished(data);

            var accounts = accountsService.GetAccounts();
            Accounts = new ObservableCollection<UserAccount>(accounts.Accounts);
        }

        protected override void SaveDocument()
        {
            if(SelectedUserAccount != null)
            {
                try
                {
                    var userAccount = JsonConvert.DeserializeObject<UserAccount>(SelectedUserAccountViewModel.UserAccountJson);
                    accountsService.SaveAccount(userAccount);
                    Accounts.Remove(SelectedUserAccount);
                    Accounts.Add(userAccount);
                    SelectedUserAccount = userAccount;
                }
                catch(Exception ex)
                {
                    ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
                }
            }
        }

        private void AddAccount(object param)
        {
            if (param == null 
                || string.IsNullOrEmpty(param.ToString()))
            {
                return;
            }

            var accountName = param.ToString();

            if(Accounts.Any( a => a.AccountName.Equals(accountName)))
            {
                ShowMessage(new Message { MessageType = MessageType.Info, Text = $"An account with the name {accountName} already exists." });
                return;
            }

            var userAccount = new UserAccount { AccountName = accountName };
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
