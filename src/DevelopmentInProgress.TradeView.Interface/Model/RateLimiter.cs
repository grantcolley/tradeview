using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Interface.Model
{
    public class RateLimiter
    {
        public bool IsEnabled { get; set; }

        public void Configure(TimeSpan duration, int count)
        {
            throw new NotImplementedException("RateLimiter.Configure(TimeSpan duration, int count)");
        }

        public Task DelayAsync(int count = 1, CancellationToken token = default(CancellationToken))
        {
            throw new NotImplementedException("RateLimiter.DelayAsync(int count = 1, CancellationToken token = default(CancellationToken))");
        }
    }
}
