using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public interface IAccountsService
    {
        Task<UserAccounts> GetAccountsAsync();
        Task<UserAccount> GetAccountAsync(string accountName);
        Task SaveAccountAsync(UserAccount userAccount);
        Task DeleteAccountAsync(UserAccount userAccount);
    }
}