using DevelopmentInProgress.Wpf.Common.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.Trading.Services
{
    public class AccountsService : IAccountsService
    {
        private string userAccountsFile;
        
        public AccountsService()
        {
            userAccountsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}.txt");
        }

        public async Task<UserAccounts> GetAccounts()
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

        public async Task<UserAccount> GetAccount(string accountName)
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

        public async Task SaveAccount(UserAccount userAccount)
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

            var wjson = JsonConvert.SerializeObject(userAccounts);

            UnicodeEncoding encoding = new UnicodeEncoding();
            char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
            using (StreamWriter writer = File.CreateText(userAccountsFile))
            {
                await writer.WriteAsync(chars, 0, chars.Length);
            }
        }

        public async Task DeleteAccount(UserAccount userAccount)
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
                    var wjson = JsonConvert.SerializeObject(userAccounts);

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
