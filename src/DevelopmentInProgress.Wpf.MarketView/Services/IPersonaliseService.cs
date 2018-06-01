using DevelopmentInProgress.Wpf.MarketView.Personalise;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.MarketView.Services
{
    public interface IAccountsService
    {
        Task<AccountPreferences> GetAccountsAsync();
        Task SaveAccountAsync(AccountPreferences accountPreferences);
    }
}