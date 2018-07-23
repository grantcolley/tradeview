using DevelopmentInProgress.Wpf.StrategyManager.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace DevelopmentInProgress.Wpf.StrategyManager.Services
{
    public class StrategyService : IStrategyService
    {
        private string userStrategiesFile;
        private object strategiesLock = new object();

        public StrategyService()
        {
            userStrategiesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}_Strategies.txt");
        }

        public List<Strategy> GetStrategies()
        {
            if (File.Exists(userStrategiesFile))
            {
                lock (strategiesLock)
                {
                    var json = File.ReadAllText(userStrategiesFile);
                    return DeserializeJson<List<Strategy>>(json);
                }
            }

            return new List<Strategy>();
        }

        public void SaveStrategy(Strategy strategy)
        {
            if(strategy == null)
            {
                return;
            }

            lock (strategiesLock)
            {
                List<Strategy> strategies;

                if (File.Exists(userStrategiesFile))
                {
                    var rjson = File.ReadAllText(userStrategiesFile);
                    strategies = DeserializeJson<List<Strategy>>(rjson);
                }
                else
                {
                    strategies = new List<Strategy>();
                }

                var dupe = strategies.FirstOrDefault(s => s.Name.Equals(strategy.Name));
                if (dupe != null)
                {
                    strategies.Remove(dupe);
                }

                strategies.Add(strategy);

                var wjson = SerializeToJson(strategies);
                File.WriteAllText(userStrategiesFile, wjson);
            }
        }

        public void DeleteStrategy(Strategy strategy)
        {
            lock (strategiesLock)
            {
                if (File.Exists(userStrategiesFile))
                {
                    var rjson = File.ReadAllText(userStrategiesFile);
                    var strategies = DeserializeJson<List<Strategy>>(rjson);

                    var remove = strategies.FirstOrDefault(s => s.Name.Equals(strategy.Name));
                    if (remove != null)
                    {
                        strategies.Remove(remove);
                        var wjson = SerializeToJson(strategies);
                        File.WriteAllText(userStrategiesFile, wjson);
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