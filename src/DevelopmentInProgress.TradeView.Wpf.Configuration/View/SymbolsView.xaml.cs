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
                if (sender is ListViewItem listViewItem)
                {
                    if (listViewItem.DataContext is Symbol symbol)
                    {
                        symbol.IsFavourite = !symbol.IsFavourite;
                    }
                }
            }
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox chkBox)
            {
                if (chkBox.DataContext is Symbol symbol)
                {
                    if (DataContext is SymbolsViewModel viewModel)
                    {
                        viewModel.UpdatePreferencesCommand.Execute(symbol);
                    }
                }
            }

            e.Handled = true;
        }
    }
}
