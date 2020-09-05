using System;
using System.Net.Http;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Manager
{
    public sealed class HttpClientManager : IHttpClientManager
    {
        private HttpClientHandler httpClientHandler;
        private HttpClient httpClient;
        private bool disposed;

        public HttpClientManager()
        {
            httpClientHandler = new HttpClientHandler();
            httpClient = new HttpClient(httpClientHandler);
        }

        public HttpClient HttpClientInstance
        {
            get { return httpClient; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if(disposed)
            {
                return;
            }

            if(disposing)
            {
                httpClientHandler.Dispose();
                httpClient.Dispose();
                httpClientHandler = null;
                httpClient = null;
            }

            disposed = true;
        }
    }
}