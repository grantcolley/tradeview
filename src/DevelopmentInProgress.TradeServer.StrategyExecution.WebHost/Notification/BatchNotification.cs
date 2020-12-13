// https://github.com/aspnet/logging/blob/dev/src/Microsoft.Extensions.Logging.AzureAppServices/Internal/BatchingLoggerProvider.cs

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification
{
    public abstract class BatchNotification<T> : IBatchNotification<T>, IDisposable
    {
        private readonly List<T> currentBatch = new List<T>();
        private BlockingCollection<T> notifyQueue;
        private Task outputTask;
        private CancellationTokenSource cancellationTokenSource;
        private bool disposed;

        protected TimeSpan Interval { get; set; }  = new TimeSpan(0, 0, 0, 1);
        protected int? QueueSize { get; set; }
        protected int? BatchSize { get; set; }

        public abstract Task NotifyAsync(IEnumerable<T> items, CancellationToken cancellationToken);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        public void AddNotification(T item)
        {
            if (!notifyQueue.IsAddingCompleted)
            {
                try
                {
                    notifyQueue.Add(item, cancellationTokenSource.Token);
                }
                catch
                {
                    //cancellation token cancelled or CompleteAdding called
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void Stop()
        {
            cancellationTokenSource.Cancel();
            notifyQueue.CompleteAdding();

            try
            {
                outputTask.Wait(Interval);
            }
            catch (TaskCanceledException)
            {
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException)
            {
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            disposed = true;
        }

        protected void Start()
        {
            if (BatchSize.HasValue
                && BatchSize <= 0)
            {
                throw new ArgumentOutOfRangeException("BatchSize", "BatchSize must be a positive number.");
            }

            if (QueueSize.HasValue
                && QueueSize <= 0)
            {
                throw new ArgumentOutOfRangeException("QueueSize", "QueueSize must be a positive number.");
            }

            if (Interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("Interval", "Interval must be longer than zero.");
            }

            notifyQueue = QueueSize == null ?
                new BlockingCollection<T>(new ConcurrentQueue<T>()) :
                new BlockingCollection<T>(new ConcurrentQueue<T>(), QueueSize.Value);

            cancellationTokenSource = new CancellationTokenSource();

            outputTask = Task.Factory.StartNew<Task>(
                ProcessNotificationQueue,
                null, cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        private async Task ProcessNotificationQueue(object state)
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                var limit = BatchSize ?? int.MaxValue;

                while (limit > 0 && notifyQueue.TryTake(out var notification))
                {
                    currentBatch.Add(notification);
                    limit--;
                }

                if (currentBatch.Count > 0)
                {
                    try
                    {
                        await NotifyAsync(currentBatch, cancellationTokenSource.Token).ConfigureAwait(false);
                    }
                    catch
                    {
                        // ignored
                    }

                    currentBatch.Clear();
                }

                await IntervalAsync(Interval, cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }

        private static Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            return Task.Delay(interval, cancellationToken);
        }
    }
}
