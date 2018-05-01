using DevelopmentInProgress.Wpf.MarketView.Personalise;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.MarketView.Services
{
    public class PersonaliseService : IPersonaliseService
    {
        private string preferencesFile;

        public PersonaliseService()
        {
            preferencesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}.txt");
        }

        public async Task<User> GetPreferencesAsync()
        {
            if (File.Exists(preferencesFile))
            {
                using (var reader = File.OpenText(preferencesFile))
                {
                    var json = await reader.ReadToEndAsync().ConfigureAwait(false);
                    return DeserializeJson<User>(json);
                }
            }

            return new User();
        }

        public async Task SavePreferences(User user)
        {
            var json = SerializeToJson(user);
            using (StreamWriter writer = File.CreateText(preferencesFile))
            {
                await writer.WriteAsync(json).ConfigureAwait(false);
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
