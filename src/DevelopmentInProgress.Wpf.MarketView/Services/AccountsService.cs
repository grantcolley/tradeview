using DevelopmentInProgress.Wpf.MarketView.Model;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace DevelopmentInProgress.Wpf.MarketView.Services
{
    public class AccountsService : IAccountsService
    {
        private string userAccountsFile;
        private object accountsLock = new object();
        
        public AccountsService()
        {
            userAccountsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}.txt");
        }

        public UserAccounts GetAccounts()
        {
            if (File.Exists(userAccountsFile))
            {
                lock (accountsLock)
                {
                    var json = File.ReadAllText(userAccountsFile);
                    return DeserializeJson<UserAccounts>(json);
                }
            }

            return new UserAccounts();
        }

        public void SaveAccount(UserAccount userAccount)
        {
            lock (accountsLock)
            {
                UserAccounts userAccounts;

                if (File.Exists(userAccountsFile))
                {
                    var rjson = File.ReadAllText(userAccountsFile);
                    userAccounts = DeserializeJson<UserAccounts>(rjson);
                }
                else
                {
                    userAccounts = new UserAccounts();
                    userAccounts.Accounts.Add(userAccount);
                }

                var wjson = SerializeToJson(userAccounts);
                File.WriteAllText(userAccountsFile, wjson);
            }
        }

        public void DeleteAccount(UserAccount userAccount)
        {
            lock(accountsLock)
            {
                if(File.Exists(userAccountsFile))
                {
                    var rjson = File.ReadAllText(userAccountsFile);
                    var userAccounts = DeserializeJson<UserAccounts>(rjson);

                    var remove = userAccounts.Accounts.FirstOrDefault(a => a.AccountName.Equals(userAccount.AccountName));
                    if(remove != null)
                    {
                        userAccounts.Accounts.Remove(remove);
                        var wjson = SerializeToJson(userAccounts);
                        File.WriteAllText(userAccountsFile, wjson);
                    }
                }
            }
        }

        private T DeserializeJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }

            var jsonSerializer = new DataContractJsonSerializer(typeof(T));
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (T)jsonSerializer.ReadObject(memoryStream);
            }
        }

        private string SerializeToJson<T>(T obj)
        {
            if (obj == null)
            {
                return null;
            }

            var jsonSerializer = new DataContractJsonSerializer(obj.GetType());
            using (var memoryStream = new MemoryStream())
            {
                jsonSerializer.WriteObject(memoryStream, obj);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }
}
