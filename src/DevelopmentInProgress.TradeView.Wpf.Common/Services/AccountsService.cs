using DevelopmentInProgress.TradeView.Data;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{ 
    public class AccountsService : IAccountsService
    {
        private ITradeViewConfigurationAccounts accounts;

        public AccountsService(ITradeViewData tradeViewData)
        {
            accounts = tradeViewData.ConfigurationData.Accounts;
        }

        public async Task<UserAccounts> GetAccountsAsync()
        {
            var result = await accounts.GetAccountsAsync();

            return new UserAccounts
            {
                Accounts = result.Accounts.Select(ua => ua.ToUserAccount()).ToList()
            };
        }

        public async Task<UserAccount> GetAccountAsync(string accountName)
        {
            var result = await accounts.GetAccountAsync(accountName);
            return result.ToUserAccount();
        }

        public Task SaveAccountAsync(UserAccount userAccount)
        {
            return accounts.SaveAccountAsync(userAccount.ToInterfaceUserAccount());
        }

        public Task DeleteAccountAsync(UserAccount userAccount)
        {
            return accounts.DeleteAccountAsync(userAccount.ToInterfaceUserAccount());
        }
    }
}
