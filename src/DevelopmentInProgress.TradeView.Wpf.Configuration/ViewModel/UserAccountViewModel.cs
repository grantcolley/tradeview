using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Command;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Configuration.Events;
using Prism.Logging;
using System;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel
{
    public class UserAccountViewModel : BaseViewModel
    {
        private UserAccount userAccount;
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
                    OnPropertyChanged("UserAccount");
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
