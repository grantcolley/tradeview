using Microsoft.Extensions.Logging;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.Logging
{
    public abstract class LoggingBase
    {
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// Initializes a new instance of the base <see cref="LoggingBase"/> class.
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILoggerFactory"/>.</param>
        protected LoggingBase(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// An instance of <see cref="ILoggerFactory"/>.
        /// </summary>
        public ILoggerFactory LoggerFactory { get { return loggerFactory; } }

        /// <summary>
        /// An instance of <see cref="ILogger"/> for the given context type."/>
        /// </summary>
        public ILogger Logger => CreateLogger(GetType());

        protected ILogger CreateLogger(Type categoryType) => LoggerFactory.CreateLogger(categoryType.FullName ?? categoryType.Name);
    }
}
