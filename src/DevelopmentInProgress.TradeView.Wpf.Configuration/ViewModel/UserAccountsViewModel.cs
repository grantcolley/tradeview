using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DevelopmentInProgress.TradeView.Wpf.Configuration.Events;
using System.Reactive.Linq;
using System.Collections.Generic;
using DevelopmentInProgress.TradeView.Wpf.Configuration.Utility;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel
{
    public class UserAccountsViewModel : DocumentViewModel
    {
        private IAccountsService accountsService;
        private ISymbolsLoader symbolsLoader;
        private UserAccount selectedUserAccount;
        private UserAccountViewModel selectedUserAccountViewModel;
        private Dictionary<string, IDisposable> observables;

        public UserAccountsViewModel(ViewModelContext viewModelContext, IAccountsService accountsService, ISymbolsLoader symbolsLoader)
            : base(viewModelContext)
        {
            this.accountsService = accountsService;
            this.symbolsLoader = symbolsLoader;

            AddAccountCommand = new ViewModelCommand(AddAccount);
            DeleteAccountCommand = new ViewModelCommand(DeleteAccount);
            CloseCommand = new ViewModelCommand(Close);

            SelectedUserAccountViewModels = new ObservableCollection<UserAccountViewModel>();

            observables = new Dictionary<string, IDisposable>();
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
                            var userAccountViewModel = new UserAccountViewModel(selectedUserAccount, Logger);
                            SelectedUserAccountViewModels.Add(userAccountViewModel);
                            SelectedUserAccountViewModel = userAccountViewModel;
                            ObserveSymbols(userAccountViewModel);
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
                if(observables.TryGetValue(userAccount.UserAccount.AccountName, out IDisposable value))
                {
                    observables.Remove(userAccount.UserAccount.AccountName);
                    value.Dispose();
                }
            }
        }

        protected async override void OnPublished(object data)
        {
            base.OnPublished(data);

            try
            {
                var accounts = await accountsService.GetAccountsAsync();
                Accounts = new ObservableCollection<UserAccount>(accounts.Accounts);
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
            }
        }

        protected async override void SaveDocument()
        {
            foreach(var userAccountViewModel in SelectedUserAccountViewModels)
            {
                try
                {
                    var userAccount = userAccountViewModel.UserAccount;

                    await accountsService.SaveAccountAsync(userAccount);

                    var account = Accounts.FirstOrDefault(a => a.AccountName.Equals(userAccount.AccountName));
                    if (account != null)
                    {
                        var index = Accounts.IndexOf(account);
                        Accounts.RemoveAt(index);
                        Accounts.Insert(index, userAccount);
                    }
                }
                catch(Exception ex)
                {
                    ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
                }
            }
        }

        private async void AddAccount(object param)
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

            try
            {
                var userAccount = new UserAccount { AccountName = accountName };
                await accountsService.SaveAccountAsync(userAccount);
                Accounts.Add(userAccount);
                Module.AddAccount(userAccount.AccountName);
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
            }
        }

        private async void DeleteAccount(object param)
        {
            var userAccount = param as UserAccount;
            if(userAccount == null)
            {
                return;
            }

            var userAccountViewModel = SelectedUserAccountViewModels.FirstOrDefault(a => a.UserAccount.AccountName.Equals(userAccount.AccountName));
            if(userAccountViewModel != null)
            {
                Close(userAccountViewModel);
            }

            try
            {
                await accountsService.DeleteAccountAsync(userAccount);
                Accounts.Remove(userAccount);
                Module.RemoveAccount(userAccount.AccountName);
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
            }
        }

        private void ObserveSymbols(UserAccountViewModel userAccountViewModel)
        {
            var symbolsObservable = Observable.FromEventPattern<UserAccountEventArgs>(
                eventHandler => userAccountViewModel.OnSymbolsNotification += eventHandler,
                eventHandler => userAccountViewModel.OnSymbolsNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);
            
            var symbolsObservableSubscription = symbolsObservable.Subscribe(async (args) =>
            {
                if (args.HasException)
                {
                    ShowMessage(new Message { Text = args.Message, MessageType = MessageType.Error });
                }
                else if (args.Value != null)
                {
                    var userAccount = args.Value as UserAccount;

                    if (userAccount != null)
                    {
                        symbolsLoader.ShowSymbols(userAccount);
                    }
                }
            });

            observables.Add(userAccountViewModel.UserAccount.AccountName, symbolsObservableSubscription);
        }
    }
}
