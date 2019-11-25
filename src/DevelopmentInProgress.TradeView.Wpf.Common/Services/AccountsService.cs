using DevelopmentInProgress.TradeView.Data;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{ 
    public class AccountsService : IAccountsService
    {
        private ITradeViewConfigurationAccounts configurationAccounts;

        public AccountsService(ITradeViewData tradeViewData)
        {
            configurationAccounts = tradeViewData.ConfigurationData.Accounts;
        }

        public async Task<UserAccounts> GetAccountsAsync()
        {
            var result = await configurationAccounts.GetAccountsAsync();

            return new UserAccounts
            {
                Accounts = result.Accounts.Select(ua => ua.ToUserAccount()).ToList()
            };
        }

        public async Task<UserAccount> GetAccountAsync(string accountName)
        {
            var result = await configurationAccounts.GetAccountAsync(accountName);
            return result.ToUserAccount();
        }

        public Task SaveAccountAsync(UserAccount userAccount)
        {
            return configurationAccounts.SaveAccountAsync(userAccount.ToInterfaceUserAccount());
        }

        public Task DeleteAccountAsync(UserAccount userAccount)
        {
            return configurationAccounts.DeleteAccountAsync(userAccount.ToInterfaceUserAccount());
        }
    }
}
