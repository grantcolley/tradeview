using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevelopmentInProgress.Wpf.Controls.TradeControl
{
    /// <summary>
    /// Interaction logic for XamlTradeControl.xaml
    /// </summary>
    public partial class XamlTradeControl : UserControl
    {
        private static readonly DependencyProperty AvailableProperty;
        private static readonly DependencyProperty IsBuyProperty;
        private static readonly DependencyProperty HasAvailableProperty;
        private static readonly DependencyProperty PercentageCommandProperty;
        private static readonly DependencyProperty PlaceTradeCommandProperty;

        static XamlTradeControl()
        {
            AvailableProperty = DependencyProperty.Register("Available", typeof(decimal), typeof(XamlTradeControl));
            IsBuyProperty = DependencyProperty.Register("IsBuy", typeof(bool), typeof(XamlTradeControl));
            HasAvailableProperty = DependencyProperty.Register("HasAvailable", typeof(bool), typeof(XamlTradeControl));
            PercentageCommandProperty = DependencyProperty.Register("PercentageCommand", typeof(ICommand), typeof(XamlTradeControl));
            PlaceTradeCommandProperty = DependencyProperty.Register("PlaceTradeCommand", typeof(ICommand), typeof(XamlTradeControl));
        }

        public XamlTradeControl()
        {
            InitializeComponent();

            DataContext = this;
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
