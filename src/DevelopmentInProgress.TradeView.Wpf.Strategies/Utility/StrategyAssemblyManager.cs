using DevelopmentInProgress.TradeView.Wpf.Common.Helpers;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies.Utility
{
    public class StrategyAssemblyManager : IStrategyAssemblyManager
    {
        private readonly ITradeHelperFactory tradeHelperFactory;
        private bool disposed;

        public StrategyAssemblyManager(ITradeHelperFactory tradeHelperFactory)
        {
            this.tradeHelperFactory = tradeHelperFactory;

            Files = new List<string>();
        }

        public string Id { get; private set; }
        public string StrategyDirectory { get; private set; }
        public List<string> Files { get; private set; }
        public object StrategyDisplayView { get; private set; }
        public object StrategyDisplayViewModel { get; private set; }

        public void Activate(Strategy strategy, Dispatcher UiDispatcher, ILoggerFacade Logger)
        {
            Id = Guid.NewGuid().ToString();

            var displayAssemblies = strategy.DisplayDependencies.Select(a => a.File).ToList();

            Download(displayAssemblies);

            var asm = Assembly.LoadFile(Path.Combine(StrategyDirectory, strategy.DisplayAssembly.DisplayName));

            var viewModel = asm.GetType(strategy.DisplayViewModelType);
            StrategyDisplayViewModel = Activator.CreateInstance(viewModel, new object[] { strategy, tradeHelperFactory.GetTradeHelper(), UiDispatcher, Logger });

            var view = asm.GetType(strategy.DisplayViewType);
            StrategyDisplayView = Activator.CreateInstance(view, new object[] { StrategyDisplayViewModel });
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (disposed)
            {
                return;
            }

            if (isDisposing)
            {
                foreach (var file in Files)
                {
                    //File.Delete(file);
                }

                disposed = true;
            }
        }

        private void Download(IEnumerable<string> files)
        {
            StrategyDirectory = Path.Combine(Directory.GetCurrentDirectory(), "strategies", Id);

            if (!Directory.Exists(StrategyDirectory))
            {
                Directory.CreateDirectory(StrategyDirectory);
            }

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var assembly = Path.Combine(StrategyDirectory, fileInfo.Name);
                File.Copy(file, assembly, true);
                Files.Add(assembly);
            }
        }
    }
}