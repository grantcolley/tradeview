using DevelopmentInProgress.Wpf.MarketView.Model;

namespace DevelopmentInProgress.Wpf.MarketView.Services
{
    public interface IAccountsService
    {
        UserAccounts GetAccounts();
        UserAccount GetAccount(string accountName);
        void SaveAccount(UserAccount userAccount);
        void DeleteAccount(UserAccount userAccount);
    }
}