using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.DecimalBox
{
    /// <summary>
    /// Interaction logic for XamlDecimalUpDown.xaml
    /// </summary>
    public partial class XamlDecimalUpDown : UserControl
    {
        public static readonly DependencyProperty DecimalValueProperty = DependencyProperty.Register("DecimalValue", typeof(decimal), typeof(XamlDecimalUpDown), new PropertyMetadata(0m));
        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(decimal), typeof(XamlDecimalUpDown), new PropertyMetadata(0m));

        public XamlDecimalUpDown()
        {
            InitializeComponent();
        }

        public decimal DecimalValue
        {
            get { return (decimal)GetValue(DecimalValueProperty); }
            set { SetValue(DecimalValueProperty, value); }
        }

        public decimal Increment
        {
            get { return (decimal)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox txt))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                return;
            }

            if (Decimal.TryParse(txt.Text, out decimal val))
            {
                if (val < 0)
                {
                    txt.Text = (val * -1).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn))
            {
                return;
            }

            if (!(btn.Tag is TextBox txt))
            {
                return;
            }

            decimal val;
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                val = 0;
            }
            else
            {
                _ = Decimal.TryParse(txt.Text, out val);
            }

            if (btn.Name.Equals("btnDown", StringComparison.Ordinal))
            {
                if (val > 0)
                {
                    txt.Text = (val - Increment).ToString(CultureInfo.InvariantCulture);
                }
            }
            else
            {
                txt.Text = (val + Increment).ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
