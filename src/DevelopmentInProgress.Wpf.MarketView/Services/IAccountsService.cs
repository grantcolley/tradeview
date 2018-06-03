using DevelopmentInProgress.Wpf.MarketView.Model;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.MarketView.Services
{
    public interface IAccountsService
    {
        UserAccounts GetAccounts();
        void SaveAccount(UserAccount userAccount);
        void DeleteAccount(UserAccount userAccount);
    }
}