using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.View
{
    /// <summary>
    /// Interaction logic for ParameterDialogView.xaml
    /// </summary>
    public partial class ParameterDialogView : Window
    {
        public ParameterDialogView(IEnumerable<Type> parameterTypes)
        {
            InitializeComponent();

            WindowStyle = WindowStyle.ToolWindow;

            StrategyTypesList.ItemsSource = parameterTypes;
        }

        public Type SelectedType { get; set; }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count.Equals(1))
            {
                SelectedType = (Type)e.AddedItems[0];
            }

            e.Handled = true;

            Close();
        }
    }
}
