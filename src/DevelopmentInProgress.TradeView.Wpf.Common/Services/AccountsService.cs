using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public class AccountsService : IAccountsService
    {
        public async Task<UserAccounts> GetAccountsAsync()
        {
            if (File.Exists(userAccountsFile))
            {
                using (var reader = File.OpenText(userAccountsFile))
                {
                    var json = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<UserAccounts>(json);
                }
            }

            return new UserAccounts();
        }

        public async Task<UserAccount> GetAccountAsync(string accountName)
        {
            if (File.Exists(userAccountsFile))
            {
                string json;
                using (var reader = File.OpenText(userAccountsFile))
                {
                    json = await reader.ReadToEndAsync();
                }

                var userAccounts = JsonConvert.DeserializeObject<UserAccounts>(json);
                var userAccount = userAccounts.Accounts.Single(a => a.AccountName.Equals(accountName));
                return userAccount;
            }

            throw new Exception($"Account {accountName} not available.");
        }

        public async Task SaveAccountAsync(UserAccount userAccount)
        {
            UserAccounts userAccounts;

            if (File.Exists(userAccountsFile))
            {
                using (var reader = File.OpenText(userAccountsFile))
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
            using (StreamWriter writer = File.CreateText(userAccountsFile))
            {
                await writer.WriteAsync(chars, 0, chars.Length);
            }
        }

        public async Task DeleteAccountAsync(UserAccount userAccount)
        {
            if (File.Exists(userAccountsFile))
            {
                UserAccounts userAccounts;

                using (var reader = File.OpenText(userAccountsFile))
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
                    using (StreamWriter writer = File.CreateText(userAccountsFile))
                    {
                        await writer.WriteAsync(chars, 0, chars.Length);
                    }
                }
            }
        }
    }
}
