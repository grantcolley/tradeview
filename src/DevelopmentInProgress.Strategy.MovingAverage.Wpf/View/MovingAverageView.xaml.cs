using DevelopmentInProgress.Strategy.MovingAverage.Wpf.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.Strategy.MovingAverage.Wpf.View
{
    /// <summary>
    /// Interaction logic for SmaView.xaml
    /// </summary>
    public partial class MovingAverageView : UserControl
    {
        public MovingAverageView(MovingAverageViewModel demoViewModel)
        {
            InitializeComponent();

            DataContext = demoViewModel;
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

        private void ResetCandlestickChartZoom(object sender, RoutedEventArgs e)
        {
            //Use the axis MinValue/MaxValue properties to specify the values to display.
            //use double.Nan to clear it.

            candlestickX.MinValue = double.NaN;
            candlestickX.MaxValue = double.NaN;
            candlestickY.MinValue = double.NaN;
            candlestickY.MaxValue = double.NaN;
        }
    }
}
