using DevelopmentInProgress.MarketView.Interface.Enums;
using DevelopmentInProgress.MarketView.Interface.Extensions;
using DevelopmentInProgress.Wpf.Common.Command;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Common.ViewModel;
using DevelopmentInProgress.Wpf.Configuration.Events;
using Newtonsoft.Json;
using Prism.Logging;
using System;
using System.Windows.Input;

namespace DevelopmentInProgress.Wpf.Configuration.ViewModel
{
    public class UserAccountViewModel : BaseViewModel
    {
        private UserAccount userAccount;
        private string userAccountJson;
        private bool disposed = false;

        public UserAccountViewModel(UserAccount userAccount, ILoggerFacade logger)
            : base(logger)
        {
            UserAccount = userAccount;

            OpenSymbolsWindowCommand = new ViewModelCommand(OpenSymbolsWindow);
        }

        public event EventHandler<UserAccountEventArgs> OnSymbolsNotification;
        
        public ICommand OpenSymbolsWindowCommand { get; set; }

        public string[] Exchanges
        {
            get { return ExchangeExtensions.Exchanges(); }
        }

        public string SelectedExchange 
        {
            get { return userAccount.Exchange.ToString(); }
            set
            {
                if(value == null)
                {
                    userAccount.Exchange = Exchange.Unknown;
                }
                else
                {
                    userAccount.Exchange = ExchangeExtensions.GetExchange(value);
                }
            }
        }

        public UserAccount UserAccount
        {
            get { return userAccount; }
            set
            {
                if (userAccount != value)
                {
                    userAccount = value;
                    if (userAccount == null)
                    {
                        UserAccountJson = string.Empty;
                    }
                    else
                    {
                        UserAccountJson = JsonConvert.SerializeObject(userAccount, Formatting.Indented);
                    }

                    OnPropertyChanged("UserAccount");
                }
            }
        }

        public string UserAccountJson
        {
            get { return userAccountJson; }
            set
            {
                if (userAccountJson != value)
                {
                    userAccountJson = value;
                    OnPropertyChanged("UserAccountJson");
                }
            }

        }

        public override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // dispose stuff...
            }

            disposed = true;
        }

        private void OpenSymbolsWindow(object param)
        {
            var onSymbolsNotification = OnSymbolsNotification;
            onSymbolsNotification?.Invoke(this, new UserAccountEventArgs { Value = userAccount });
        }
    }
}
