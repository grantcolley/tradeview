using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Common.ViewModel;
using Newtonsoft.Json;
using Prism.Logging;

namespace DevelopmentInProgress.Wpf.Trading.ViewModel
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
    }
}
