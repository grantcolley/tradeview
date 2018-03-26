using System;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.Wpf.Controls.DecimalBox
{
    partial class DecimalBox
    {
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            if(txt == null)
            {
                return;
            }

            if(string.IsNullOrWhiteSpace(txt.Text))
            {
                return;
            }

            decimal val;
            if(Decimal.TryParse(txt.Text, out val))
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
            if(btn == null)
            {
                return;
            }

            var txt = btn.Tag as TextBox;
            if(txt == null)
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

            if(btn.Name.Equals("btnDown"))
            {
                if (val > 0)
                {
                    txt.Text = (--val).ToString();
                }
            }
            else
            {
                txt.Text = (++val).ToString();
            }
        }
    }
}