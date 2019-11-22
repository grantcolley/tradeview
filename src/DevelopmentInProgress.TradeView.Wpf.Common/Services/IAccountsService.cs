using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public interface IAccountsService
    {
        Task<UserAccounts> GetAccounts();
        Task<UserAccount> GetAccount(string accountName);
        Task SaveAccount(UserAccount userAccount);
        Task DeleteAccount(UserAccount userAccount);
    }
}