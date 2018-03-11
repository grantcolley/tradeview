using DevelopmentInProgress.Wpf.MarketView.Personalise;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.MarketView.Services
{
    public interface IPersonaliseService
    {
        Task<User> GetPreferencesAsync();
        void SavePreferences(User user);
    }
}