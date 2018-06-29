using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Interface.TradeStrategy
{
    public class StrategyRunnerClient
    {
        public Task<HttpResponseMessage> PostAsync(string requestUri, string jsonSerializedStrategy, IEnumerable<string> libraries)
        {
            var client = new HttpClient();
            var multipartFormDataContent = new MultipartFormDataContent();

            multipartFormDataContent.Add(new StringContent(jsonSerializedStrategy, Encoding.UTF8, "application/json"), "strategy");

            foreach (var file in libraries)
            {
                var fileInfo = new FileInfo(file);
                var fileStream = File.OpenRead(file);
                using (var br = new BinaryReader(fileStream))
                {
                    var byteArrayContent = new ByteArrayContent(br.ReadBytes((int)fileStream.Length));
                    multipartFormDataContent.Add(byteArrayContent, fileInfo.Name, fileInfo.FullName);
                }
            }

            return client.PostAsync(requestUri, multipartFormDataContent);
        }
    }
}
