using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Data.File
{
    public class TradeViewConfigurationAccountsFile : ITradeViewConfigurationAccounts
    {
        public Task DeleteAccountAsync(UserAccount userAccount)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserAccount> GetAccountAsync(string accountName)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserAccounts> GetAccountsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task SaveAccountAsync(UserAccount userAccount)
        {
            throw new System.NotImplementedException();
        }
    }
}
