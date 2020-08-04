using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.TradeControl
{
    /// <summary>
    /// Interaction logic for XamlTradeControl.xaml
    /// </summary>
    public partial class XamlTradeControl : UserControl
    {
        private static readonly DependencyProperty CurrencyProperty = 
            DependencyProperty.Register("Currency", typeof(string), typeof(XamlTradeControl));

        private static readonly DependencyProperty AvailableProperty = 
            DependencyProperty.Register("Available", typeof(decimal), typeof(XamlTradeControl));

        private static readonly DependencyProperty IsBuyProperty = 
            DependencyProperty.Register("IsBuy", typeof(bool), typeof(XamlTradeControl));

        private static readonly DependencyProperty HasAvailableProperty = 
            DependencyProperty.Register("HasAvailable", typeof(bool), typeof(XamlTradeControl));

        private static readonly DependencyProperty PercentageCommandProperty = 
            DependencyProperty.Register("PercentageCommand", typeof(ICommand), typeof(XamlTradeControl));

        private static readonly DependencyProperty PlaceTradeCommandProperty = 
            DependencyProperty.Register("PlaceTradeCommand", typeof(ICommand), typeof(XamlTradeControl));

        public XamlTradeControl()
        {
            InitializeComponent();
        }

        public string Currency
        {
            get { return (string)GetValue(CurrencyProperty); }
            set { SetValue(CurrencyProperty, value); }
        }

        public decimal Available
        {
            get { return (decimal)GetValue(AvailableProperty); }
            set { SetValue(AvailableProperty, value); }
        }

        public bool IsBuy
        {
            get { return (bool)GetValue(IsBuyProperty); }
            set { SetValue(IsBuyProperty, value); }
        }

        public bool HasAvailable
        {
            get { return (bool)GetValue(HasAvailableProperty); }
            set { SetValue(HasAvailableProperty, value); }
        }

        public ICommand PercentageCommand
        {
            get { return (ICommand)GetValue(PercentageCommandProperty); }
            set { SetValue(PercentageCommandProperty, value); }
        }

        public ICommand PlaceTradeCommand
        {
            get { return (ICommand)GetValue(PlaceTradeCommandProperty); }
            set { SetValue(PlaceTradeCommandProperty, value); }
        }
    }
}
