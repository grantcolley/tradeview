using System;
using System.Net.Http;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Manager
{
    public interface IHttpClientManager : IDisposable
    {
        HttpClient HttpClientInstance { get; }
    }
}
