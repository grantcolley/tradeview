using LiveCharts;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.View
{
    /// <summary>
    /// Interaction logic for SymbolView.xaml
    /// </summary>
    public partial class SymbolView : UserControl
    {
        public SymbolView()
        {
            InitializeComponent();
        }

        private void ResetTradesChartZoom(object sender, RoutedEventArgs e)
        {
            //Use the axis MinValue/MaxValue properties to specify the values to display.
            //use double.Nan to clear it.

            tradeX.MinValue = double.NaN;
            tradeX.MaxValue = double.NaN;
            tradeY.MinValue = double.NaN;
            tradeY.MaxValue = double.NaN;
        }

        private void ResetOrderBookChartZoom(object sender, RoutedEventArgs e)
        {
            //Use the axis MinValue/MaxValue properties to specify the values to display.
            //use double.Nan to clear it.

            orderBookX.MinValue = double.NaN;
            orderBookX.MaxValue = double.NaN;
            orderBookY.MinValue = double.NaN;
            orderBookY.MaxValue = double.NaN;
        }

        private void ResetOrderBookAggregateChartZoom(object sender, RoutedEventArgs e)
        {
            //Use the axis MinValue/MaxValue properties to specify the values to display.
            //use double.Nan to clear it.

            orderBookAggregateX.MinValue = double.NaN;
            orderBookAggregateX.MaxValue = double.NaN;
            orderBookAggregateY.MinValue = double.NaN;
            orderBookAggregateY.MaxValue = double.NaN;
        }
    }
}
