using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.View
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

        private void ListViewItemPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                var symbol = ((ListViewItem)sender).DataContext as Symbol;
                if (symbol != null)
                {
                    symbol.IsFavourite = !symbol.IsFavourite;
                }
            }
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            var symbol = ((CheckBox)sender).DataContext as Symbol;
            if(symbol != null)
            {
                var viewModel = DataContext as SymbolsViewModel;
                if(viewModel != null)
                {
                    viewModel.UpdatePreferencesCommand.Execute(symbol);
                }
            }

            e.Handled = true;
        }
    }
}
