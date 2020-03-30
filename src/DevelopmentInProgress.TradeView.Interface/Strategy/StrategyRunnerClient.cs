using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Interface.Strategy
{
    public class StrategyRunnerClient
    {
        public Task<HttpResponseMessage> PostAsync(string requestUri, string jsonSerializedStrategy, IEnumerable<string> libraries)
        {
            using (var client = new HttpClient())
            {
                using (var multipartFormDataContent = new MultipartFormDataContent())
                {

                    multipartFormDataContent.Add(new StringContent(jsonSerializedStrategy, Encoding.UTF8, "application/json"), "strategy");

                    foreach (var file in libraries)
                    {
                        var fileInfo = new FileInfo(file);
                        using (var fileStream = File.OpenRead(file))
                        {
                            using (var br = new BinaryReader(fileStream))
                            {
                                using (var byteArrayContent = new ByteArrayContent(br.ReadBytes((int)fileStream.Length)))
                                {
                                    multipartFormDataContent.Add(byteArrayContent, fileInfo.Name, fileInfo.FullName);
                                }
                            }
                        }
                    }

                    return client.PostAsync(requestUri, multipartFormDataContent);
                }
            }
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, string jsonSerializedStrategyParameters)
        {
            using (var client = new HttpClient())
            {
                using (var multipartFormDataContent = new MultipartFormDataContent())
                {
                    multipartFormDataContent.Add(new StringContent(jsonSerializedStrategyParameters, Encoding.UTF8, "application/json"), "strategyparameters");
                    return client.PostAsync(requestUri, multipartFormDataContent);
                }
            }
        }
    }
}