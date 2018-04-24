using DevelopmentInProgress.Wpf.MarketView.Personalise;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.MarketView.Services
{
    public interface IPersonaliseService
    {
        Task<User> GetPreferencesAsync();
        Task SavePreferences(User user);
    }
}