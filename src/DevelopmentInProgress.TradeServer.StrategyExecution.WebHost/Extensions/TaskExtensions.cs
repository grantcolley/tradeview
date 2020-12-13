using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Warning: Exceptions will be swallowed. The function being executed must deal with exceptions.
        /// </summary>
        /// <param name="task">The task to 'fire and forget'</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1030:Use events where appropriate")]
        public static async void FireAndForget(this Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

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
