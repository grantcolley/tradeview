using DevelopmentInProgress.TradeView.Data;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{ 
    public class AccountsService : IAccountsService
    {
        private readonly ITradeViewConfigurationAccounts configurationAccounts;

        public AccountsService(ITradeViewConfigurationAccounts configurationAccounts)
        {
            this.configurationAccounts = configurationAccounts;
        }

        public async Task<UserAccounts> GetAccountsAsync()
        {
            var result = await configurationAccounts.GetAccountsAsync().ConfigureAwait(false);

            var userAccounts = new UserAccounts();
            userAccounts.Accounts.AddRange(result.Accounts.Select(ua => ua.ToUserAccount()).ToList());
            return userAccounts;
        }

        public async Task<UserAccount> GetAccountAsync(string accountName)
        {
            var result = await configurationAccounts.GetAccountAsync(accountName).ConfigureAwait(false);
            return result.ToUserAccount();
        }

        public Task SaveAccountAsync(UserAccount userAccount)
        {
            return configurationAccounts.SaveAccountAsync(userAccount.ToCoreUserAccount());
        }

        public Task DeleteAccountAsync(UserAccount userAccount)
        {
            return configurationAccounts.DeleteAccountAsync(userAccount.ToCoreUserAccount());
        }
    }
}
