using DevelopmentInProgress.Strategy.Demo.Wpf.ViewModel;
using System.Windows.Controls;

namespace DevelopmentInProgress.Strategy.Demo.Wpf.View
{
    /// <summary>
    /// Interaction logic for SmaView.xaml
    /// </summary>
    public partial class DemoView : UserControl
    {
        public DemoView(DemoViewModel demoViewModel)
        {
            InitializeComponent();

            DataContext = demoViewModel;
        }
    }
}
