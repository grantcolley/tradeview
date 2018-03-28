using System;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.Wpf.Controls.DecimalBox
{
    /// <summary>
    /// Interaction logic for XamlDecimalUpDown.xaml
    /// </summary>
    public partial class XamlDecimalUpDown : UserControl
    {
        public static readonly DependencyProperty DecimalValueProperty = DependencyProperty.Register("DecimalValue", typeof(decimal), typeof(XamlDecimalUpDown), new PropertyMetadata(0m));

        public XamlDecimalUpDown()
        {
            InitializeComponent();
        }

        public decimal DecimalValue
        {
            get { return (decimal)GetValue(DecimalValueProperty); }
            set { SetValue(DecimalValueProperty, value); }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                return;
            }

            decimal val;
            if (Decimal.TryParse(txt.Text, out val))
            {
                if (val < 0)
                {
                    txt.Text = (val * -1).ToString();
                }
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
            {
                return;
            }

            var txt = btn.Tag as TextBox;
            if (txt == null)
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
                Decimal.TryParse(txt.Text, out val);
            }

            if (btn.Name.Equals("btnDown"))
            {
                if (val > 0)
                {
                    txt.Text = val.Decrement().ToString();
                }
            }
            else
            {
                txt.Text = val.Increment().ToString();
            }
        }
    }
}
