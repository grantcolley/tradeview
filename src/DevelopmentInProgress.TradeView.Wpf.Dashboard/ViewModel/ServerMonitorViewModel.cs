using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel
{
    public class ServerMonitorViewModel : DocumentViewModel
    {
        public ServerMonitorViewModel(ViewModelContext viewModelContext)
            : base(viewModelContext)
        {
        }

        protected async override void OnPublished(object data)
        {
            // Do stuff here...
        }
    }
}