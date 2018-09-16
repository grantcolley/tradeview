using DevelopmentInProgress.Wpf.Common.Events;
using DevelopmentInProgress.Wpf.Common.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.MarketView.Events
{
    public class OrdersEventArgs : EventArgsBase<IEnumerable<Order>>
    {
    }
}