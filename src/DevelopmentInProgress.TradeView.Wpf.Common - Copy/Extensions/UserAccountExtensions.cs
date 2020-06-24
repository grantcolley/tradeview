using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class UserAccountExtensions
    {
        public static UserAccount ToUserAccount(this Core.Model.UserAccount ua)
        {
            var userAccount = new UserAccount
            {
                AccountName = ua.AccountName,
                ApiKey = ua.ApiKey,
                ApiSecret = ua.ApiSecret,
                ApiPassPhrase = ua.ApiPassPhrase,
                Exchange = ua.Exchange,
                Preferences = new Preferences
                {
                    SelectedSymbol = ua.Preferences.SelectedSymbol,
                    ShowAggregateTrades = ua.Preferences.ShowAggregateTrades,
                    TradeLimit = ua.Preferences.TradeLimit,
                    TradesChartDisplayCount = ua.Preferences.TradesChartDisplayCount,
                    TradesDisplayCount = ua.Preferences.TradesDisplayCount,
                    OrderBookLimit = ua.Preferences.OrderBookLimit,
                    OrderBookChartDisplayCount = ua.Preferences.OrderBookChartDisplayCount,
                    OrderBookDisplayCount = ua.Preferences.OrderBookDisplayCount,
                    FavouriteSymbols = new ObservableCollection<string>(ua.Preferences.FavouriteSymbols)
                }
            };

            return userAccount;
        }

        public static Core.Model.UserAccount ToCoreUserAccount(this UserAccount ua)
        {
            var userAccount = new Core.Model.UserAccount
            {
                AccountName = ua.AccountName,
                ApiKey = ua.ApiKey,
                ApiSecret = ua.ApiSecret,
                ApiPassPhrase = ua.ApiPassPhrase,
                Exchange = ua.Exchange,
                Preferences = new Core.Model.Preferences
                {
                    SelectedSymbol = ua.Preferences.SelectedSymbol,
                    ShowAggregateTrades = ua.Preferences.ShowAggregateTrades,
                    TradeLimit = ua.Preferences.TradeLimit,
                    TradesChartDisplayCount = ua.Preferences.TradesChartDisplayCount,
                    TradesDisplayCount = ua.Preferences.TradesDisplayCount,
                    OrderBookLimit = ua.Preferences.OrderBookLimit,
                    OrderBookChartDisplayCount = ua.Preferences.OrderBookChartDisplayCount,
                    OrderBookDisplayCount = ua.Preferences.OrderBookDisplayCount
                }
            };

            userAccount.Preferences.FavouriteSymbols.AddRange(ua.Preferences.FavouriteSymbols.ToList());

            return userAccount;
        }
    }
}
