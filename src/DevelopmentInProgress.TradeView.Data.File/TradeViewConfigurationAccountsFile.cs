using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Model;
using Newtonsoft.Json;

namespace DevelopmentInProgress.TradeView.Data.File
{
    public class TradeViewConfigurationAccountsFile : ITradeViewConfigurationAccounts
    {
        private string userAccountsFile;

        public TradeViewConfigurationAccountsFile()
        {
            userAccountsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}_Accounts.txt");
        }

        public async Task DeleteAccountAsync(UserAccount userAccount)
        {
            if (System.IO.File.Exists(userAccountsFile))
            {
                UserAccounts userAccounts;

                using (var reader = System.IO.File.OpenText(userAccountsFile))
                {
                    var rjson = await reader.ReadToEndAsync();
                    userAccounts = JsonConvert.DeserializeObject<UserAccounts>(rjson);
                }

                var remove = userAccounts.Accounts.FirstOrDefault(a => a.AccountName.Equals(userAccount.AccountName));
                if (remove != null)
                {
                    userAccounts.Accounts.Remove(remove);
                    var wjson = JsonConvert.SerializeObject(userAccounts, Formatting.Indented);

                    UnicodeEncoding encoding = new UnicodeEncoding();
                    char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
                    using (StreamWriter writer = System.IO.File.CreateText(userAccountsFile))
                    {
                        await writer.WriteAsync(chars, 0, chars.Length);
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
                    json = await reader.ReadToEndAsync();
                }

                var userAccounts = JsonConvert.DeserializeObject<UserAccounts>(json);
                var userAccount = userAccounts.Accounts.Single(a => a.AccountName.Equals(accountName));
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
                    var json = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<UserAccounts>(json);
                }
            }

            return new UserAccounts
            {
                Accounts = new List<UserAccount>
                { 
                    GetDemoAccount() 
                }
            };
        }

        public async Task SaveAccountAsync(UserAccount userAccount)
        {
            UserAccounts userAccounts;

            if (System.IO.File.Exists(userAccountsFile))
            {
                using (var reader = System.IO.File.OpenText(userAccountsFile))
                {
                    var rjson = await reader.ReadToEndAsync();
                    userAccounts = JsonConvert.DeserializeObject<UserAccounts>(rjson);
                }
            }
            else
            {
                userAccounts = new UserAccounts();
            }

            var dupe = userAccounts.Accounts.FirstOrDefault(a => a.AccountName.Equals(userAccount.AccountName));
            if (dupe != null)
            {
                userAccounts.Accounts.Remove(dupe);
            }

            userAccounts.Accounts.Add(userAccount);

            var wjson = JsonConvert.SerializeObject(userAccounts, Formatting.Indented);

            UnicodeEncoding encoding = new UnicodeEncoding();
            char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
            using (StreamWriter writer = System.IO.File.CreateText(userAccountsFile))
            {
                await writer.WriteAsync(chars, 0, chars.Length);
            }
        }

        private UserAccount GetDemoAccount()
        {
            return new UserAccount
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
                    OrderBookChartDisplayCount = 15,
                    FavouriteSymbols = new List<string> { "BTCUSDT", "ETHBTC", "ETHUSDT" }
                }
            };
        }
    }
}
