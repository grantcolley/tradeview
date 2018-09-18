using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Api.Binance
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Warning: Exceptions will be swallowed. The function being executed must deal with exceptions.
        /// </summary>
        /// <param name="task">The task to 'fire and forget'</param>
        public static async void FireAndForget(this Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception)
            {
                // intentionally swallow.
            }
        }
    }
}
