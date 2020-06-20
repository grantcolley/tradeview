using DevelopmentInProgress.TradeView.Core.Model;
using DevelopmentInProgress.TradeView.Data.File.Model;
using System;

namespace DevelopmentInProgress.TradeView.Data.File.Extensions
{
    internal static class UserAccountExtensions
    {
        internal static UserAccountPreferencesData ConvertToUserAccountPreferencesData(this UserAccount userAccount)
        {
            if (userAccount == null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }

            var userAccountPreferencesData = new UserAccountPreferencesData
            {
                AccountName = userAccount.AccountName,
                Exchange = userAccount.Exchange,
                ApiKey = userAccount.ApiKey,
                ApiSecret = userAccount.ApiSecret,
                ApiPassPhrase = userAccount.ApiPassPhrase,
                SelectedSymbol = userAccount.Preferences.SelectedSymbol,
                ShowAggregateTrades = userAccount.Preferences.ShowAggregateTrades,
                TradeLimit = userAccount.Preferences.TradeLimit,
                TradesChartDisplayCount = userAccount.Preferences.TradesChartDisplayCount,
                TradesDisplayCount = userAccount.Preferences.TradesDisplayCount,
                OrderBookLimit = userAccount.Preferences.OrderBookLimit,
                OrderBookChartDisplayCount = userAccount.Preferences.OrderBookChartDisplayCount,
                OrderBookDisplayCount = userAccount.Preferences.OrderBookDisplayCount
            };

            userAccountPreferencesData.FavouriteSymbols.AddRange(userAccount.Preferences.FavouriteSymbols);

            return userAccountPreferencesData;
        }

        internal static UserAccount ConvertToUserAccount(this UserAccountPreferencesData userAccountPreferencesData)
        {
            if (userAccountPreferencesData == null)
            {
                throw new ArgumentNullException(nameof(userAccountPreferencesData));
            }

            var userAccount = new UserAccount
            {
                AccountName = userAccountPreferencesData.AccountName,
                Exchange = userAccountPreferencesData.Exchange,
                ApiKey = userAccountPreferencesData.ApiKey,
                ApiSecret = userAccountPreferencesData.ApiSecret,
                ApiPassPhrase = userAccountPreferencesData.ApiPassPhrase,
                Preferences = new Preferences
                {
                    SelectedSymbol = userAccountPreferencesData.SelectedSymbol,
                    ShowAggregateTrades = userAccountPreferencesData.ShowAggregateTrades,
                    TradeLimit = userAccountPreferencesData.TradeLimit,
                    TradesChartDisplayCount = userAccountPreferencesData.TradesChartDisplayCount,
                    TradesDisplayCount = userAccountPreferencesData.TradesDisplayCount,
                    OrderBookLimit = userAccountPreferencesData.OrderBookLimit,
                    OrderBookChartDisplayCount = userAccountPreferencesData.OrderBookChartDisplayCount,
                    OrderBookDisplayCount = userAccountPreferencesData.OrderBookDisplayCount
                }
            };

            userAccount.Preferences.FavouriteSymbols.AddRange(userAccountPreferencesData.FavouriteSymbols);

            return userAccount;
        }
    }
}
