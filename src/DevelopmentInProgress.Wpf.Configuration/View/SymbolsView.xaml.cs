using DevelopmentInProgress.Wpf.Configuration.ViewModel;
using System.Windows;

namespace DevelopmentInProgress.Wpf.Configuration.View
{
    /// <summary>
    /// Interaction logic for SymbolsView.xaml
    /// </summary>
    public partial class SymbolsView : Window
    {
        public SymbolsView(SymbolsViewModel symbolsViewModel)
        {
            InitializeComponent();

            DataContext = symbolsViewModel;
        }
    }
}
