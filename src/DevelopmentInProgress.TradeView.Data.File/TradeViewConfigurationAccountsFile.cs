using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Model;
using DevelopmentInProgress.TradeView.Data.File.Extensions;
using DevelopmentInProgress.TradeView.Data.File.Model;
using Newtonsoft.Json;

namespace DevelopmentInProgress.TradeView.Data.File
{
    public class TradeViewConfigurationAccountsFile : ITradeViewConfigurationAccounts
    {
        private readonly string userAccountsFile;

        public TradeViewConfigurationAccountsFile()
        {
            userAccountsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.MachineName}_Accounts.txt");
        }

        public async Task DeleteAccountAsync(UserAccount userAccount)
        {
            if (System.IO.File.Exists(userAccountsFile))
            {
                UserAccountsPreferencesData userAccountsPreferencesData;

                using (var reader = System.IO.File.OpenText(userAccountsFile))
                {
                    var rjson = await reader.ReadToEndAsync().ConfigureAwait(false);
                    userAccountsPreferencesData = JsonConvert.DeserializeObject<UserAccountsPreferencesData>(rjson);
                }

                var remove = userAccountsPreferencesData.Accounts.FirstOrDefault(a => a.AccountName.Equals(userAccount.AccountName, StringComparison.Ordinal));
                if (remove != null)
                {
                    userAccountsPreferencesData.Accounts.Remove(remove);
                    var wjson = JsonConvert.SerializeObject(userAccountsPreferencesData, Formatting.Indented);

                    UnicodeEncoding encoding = new UnicodeEncoding();
                    char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
                    using (StreamWriter writer = System.IO.File.CreateText(userAccountsFile))
                    {
                        await writer.WriteAsync(chars, 0, chars.Length).ConfigureAwait(false);
                    }
                }
            }
        }

        public async Task<UserAccount> GetAccountAsync(string accountName)
        {
            if (System.IO.File.Exists(userAccountsFile))
            {
                string json;
                using (var reader = System.IO.File.OpenText(userAccountsFile))
                {
                    json = await reader.ReadToEndAsync().ConfigureAwait(false);
                }

                var userAccountsPreferencesData = JsonConvert.DeserializeObject<UserAccountsPreferencesData>(json);
                var userAccountPreferencesData = userAccountsPreferencesData.Accounts.Single(a => a.AccountName.Equals(accountName, StringComparison.Ordinal));
                var userAccount = userAccountPreferencesData.ConvertToUserAccount();
                return userAccount;
            }

            return GetDemoAccount();
        }

        public async Task<UserAccounts> GetAccountsAsync()
        {
            if (System.IO.File.Exists(userAccountsFile))
            {
                using (var reader = System.IO.File.OpenText(userAccountsFile))
                {
                    var json = await reader.ReadToEndAsync().ConfigureAwait(true);
                    var userAccountsPreferencesData = JsonConvert.DeserializeObject<UserAccountsPreferencesData>(json);
                    var accounts = userAccountsPreferencesData.Accounts.Select(ua => ua.ConvertToUserAccount()).ToList();
                    var userAccounts = new UserAccounts();
                    userAccounts.Accounts.AddRange(accounts);
                    return userAccounts;
                }
            }

            var demoUserAccounts = new UserAccounts();
            demoUserAccounts.Accounts.Add(GetDemoAccount());
            return demoUserAccounts;
        }

        public async Task SaveAccountAsync(UserAccount userAccount)
        {
            UserAccountsPreferencesData userAccountsPreferencesData;

            if (System.IO.File.Exists(userAccountsFile))
            {
                using (var reader = System.IO.File.OpenText(userAccountsFile))
                {
                    var rjson = await reader.ReadToEndAsync().ConfigureAwait(false);
                    userAccountsPreferencesData = JsonConvert.DeserializeObject<UserAccountsPreferencesData>(rjson);
                }
            }
            else
            {
                userAccountsPreferencesData = new UserAccountsPreferencesData();
            }

            var dupe = userAccountsPreferencesData.Accounts.FirstOrDefault(a => a.AccountName.Equals(userAccount.AccountName, StringComparison.Ordinal));
            if (dupe != null)
            {
                userAccountsPreferencesData.Accounts.Remove(dupe);
            }

            userAccountsPreferencesData.Accounts.Add(userAccount.ConvertToUserAccountPreferencesData());

            var wjson = JsonConvert.SerializeObject(userAccountsPreferencesData, Formatting.Indented);

            UnicodeEncoding encoding = new UnicodeEncoding();
            char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
            using (StreamWriter writer = System.IO.File.CreateText(userAccountsFile))
            {
                await writer.WriteAsync(chars, 0, chars.Length).ConfigureAwait(false);
            }
        }

        private static UserAccount GetDemoAccount()
        {
            var userAccount = new UserAccount
            {
                AccountName = "Demo Account",
                Exchange = Exchange.Binance,
                Preferences = new Preferences
                {
                    SelectedSymbol = "ETHBTC",
                    TradeLimit = 500,
                    TradesChartDisplayCount = 500,
                    TradesDisplayCount = 18,
                    OrderBookDisplayCount = 9,
                    OrderBookChartDisplayCount = 15
                }
            };

            userAccount.Preferences.FavouriteSymbols.AddRange(new string[] { "BTCUSDT", "ETHBTC", "ETHUSDT" });

            return userAccount;
        }
    }
}
