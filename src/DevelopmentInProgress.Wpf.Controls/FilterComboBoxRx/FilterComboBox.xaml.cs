using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.Wpf.Controls.FilterComboBoxRx
{
    /// <summary>
    /// Interaction logic for FilterComboBox.xaml
    /// </summary>
    public partial class FilterComboBox : UserControl
    {
        private static readonly DependencyProperty FilterFieldNameProperty;
        private static readonly DependencyProperty ItemsSourceProperty;

        static FilterComboBox()
        {
            FilterFieldNameProperty = DependencyProperty.Register("FilterFieldName", typeof(string), typeof(FilterComboBox));
            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(FilterComboBox));
        }

        public FilterComboBox()
        {
            InitializeComponent();            
        }


    }
}
