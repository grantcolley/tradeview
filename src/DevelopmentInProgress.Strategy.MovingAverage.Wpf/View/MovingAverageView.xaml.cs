using DevelopmentInProgress.Strategy.MovingAverage.Wpf.ViewModel;
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
    }
}
