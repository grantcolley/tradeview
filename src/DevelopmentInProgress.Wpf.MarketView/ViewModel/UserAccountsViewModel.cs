using DevelopmentInProgress.Wpf.Controls.Messaging;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class UserAccountsViewModel : DocumentViewModel
    {
        private IAccountsService accountsService;
        private UserAccount selectedUserAccount;
        private string selectedUserAccountJson;

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

        public UserAccount SelectedUserAccount
        {
            get { return selectedUserAccount; }
            set
            {
                if (selectedUserAccount != value)
                {
                    selectedUserAccount = value;
                    if (selectedUserAccount == null)
                    {
                        SelectedUserAccountJson = string.Empty;
                    }
                    else
                    {
                        SelectedUserAccountJson = JsonConvert.SerializeObject(selectedUserAccount, Formatting.Indented);
                    }

                    OnPropertyChanged("SelectedUserAccount");
                }
            }
        }

        public string SelectedUserAccountJson
        {
            get { return selectedUserAccountJson; }
            set
            {
                if (selectedUserAccountJson != value)
                {
                    selectedUserAccountJson = value;
                    OnPropertyChanged("SelectedUserAccountJson");
                }
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
                    var userAccount = JsonConvert.DeserializeObject<UserAccount>(selectedUserAccountJson);
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
