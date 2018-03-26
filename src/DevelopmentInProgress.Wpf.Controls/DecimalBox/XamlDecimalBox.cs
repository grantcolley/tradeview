using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.Wpf.Controls.DecimalBox
{
    public class XamlDecimalBox : Control
    {
        private static readonly DependencyProperty ValueProperty;

        static XamlDecimalBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XamlDecimalBox), new FrameworkPropertyMetadata(typeof(XamlDecimalBox)));

            ValueProperty = DependencyProperty.Register("Value", typeof(decimal), typeof(XamlDecimalBox), new PropertyMetadata(0m));
        }

        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}