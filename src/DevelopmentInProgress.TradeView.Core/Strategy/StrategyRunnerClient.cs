using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Core.Strategy
{
    public class StrategyRunnerClient
    {
        public async Task<HttpResponseMessage> PostAsync(string requestUri, string jsonSerializedStrategy, IEnumerable<string> libraries)
        {
            if (libraries == null)
            {
                throw new ArgumentNullException(nameof(libraries));
            }

            var byteArrayContents = new List<ByteArrayContent>();

            try
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
                                    var byteArrayContent = new ByteArrayContent(br.ReadBytes((int)fileStream.Length));
                                    multipartFormDataContent.Add(byteArrayContent, fileInfo.Name, fileInfo.FullName);
                                    byteArrayContents.Add(byteArrayContent);
                                }
                            }
                        }

                        return await client.PostAsync(requestUri, multipartFormDataContent).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                foreach (var byteArrayContent in byteArrayContents)
                {
                    byteArrayContent.Dispose();
                }
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, string jsonSerializedStrategyParameters)
        {
            var stringContent = new StringContent(jsonSerializedStrategyParameters, Encoding.UTF8, "application/json");

            try
            {
                using (var client = new HttpClient())
                {
                    using (var multipartFormDataContent = new MultipartFormDataContent())
                    {
                        multipartFormDataContent.Add(stringContent, "strategyparameters");
                        return await client.PostAsync(requestUri, multipartFormDataContent).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                stringContent.Dispose();
            }
        }
    }
}