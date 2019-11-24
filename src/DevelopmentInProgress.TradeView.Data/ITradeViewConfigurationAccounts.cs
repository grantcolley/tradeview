using DevelopmentInProgress.TradeView.Interface.Model;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Data
{
    public interface ITradeViewConfigurationAccounts
    {
        Task<UserAccounts> GetAccountsAsync();
        Task<UserAccount> GetAccountAsync(string accountName);
        Task SaveAccountAsync(UserAccount userAccount);
        Task DeleteAccountAsync(UserAccount userAccount);
    }
}
