using DevelopmentInProgress.Socket.Messages;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Strategy
{
    public class StrategyNotificationPublisherContext : IStrategyNotificationPublisherContext
    {
        private readonly StrategyNotificationHub notificationHub;

        public StrategyNotificationPublisherContext(StrategyNotificationHub notificationHub)
        {
            this.notificationHub = notificationHub;
        }

        public async Task PublishCustomNotificationsAsync(string strategyName, string methodName, IEnumerable<StrategyNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = strategyName, MessageType = MessageType.SendToChannel, MethodName = methodName, Data = json };
            await notificationHub.SendMessageToChannelAsync(strategyName, msg).ConfigureAwait(false);
        }

        public async Task PublishNotificationsAsync(string strategyName, IEnumerable<StrategyNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = strategyName, MessageType = MessageType.SendToChannel, MethodName = "Notification", Data = json };
            await notificationHub.SendMessageToChannelAsync(strategyName, msg).ConfigureAwait(false);
        }

        public async Task PublishTradesAsync(string strategyName, IEnumerable<StrategyNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = strategyName, MessageType = MessageType.SendToChannel, MethodName = "Trade", Data = json };
            await notificationHub.SendMessageToChannelAsync(strategyName, msg).ConfigureAwait(false);
        }

        public async Task PublishCandlesticksAsync(string strategyName, IEnumerable<StrategyNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = strategyName, MessageType = MessageType.SendToChannel, MethodName = "Candlesticks", Data = json };
            await notificationHub.SendMessageToChannelAsync(strategyName, msg).ConfigureAwait(false);
        }

        public async Task PublishStatisticsAsync(string strategyName, IEnumerable<StrategyNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = strategyName, MessageType = MessageType.SendToChannel, MethodName = "Statistics", Data = json };
            await notificationHub.SendMessageToChannelAsync(strategyName, msg).ConfigureAwait(false);
        }

        public async Task PublishOrderBookAsync(string strategyName, IEnumerable<StrategyNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = strategyName, MessageType = MessageType.SendToChannel, MethodName = "OrderBook", Data = json };
            await notificationHub.SendMessageToChannelAsync(strategyName, msg).ConfigureAwait(false);
        }

        public async Task PublishAccountInfoAsync(string strategyName, IEnumerable<StrategyNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = strategyName, MessageType = MessageType.SendToChannel, MethodName = "AccountInfo", Data = json };
            await notificationHub.SendMessageToChannelAsync(strategyName, msg).ConfigureAwait(false);
        }

        public async Task PublishParameterUpdateAsync(string strategyName, IEnumerable<StrategyNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = strategyName, MessageType = MessageType.SendToChannel, MethodName = "ParameterUpdate", Data = json };
            await notificationHub.SendMessageToChannelAsync(strategyName, msg).ConfigureAwait(false);
        }
    }
}
