using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DevelopmentInProgress.Wpf.Controls.FilterComboBox
{
    /// <summary>
    /// Interaction logic for FilterComboBox.xaml
    /// </summary>
    public partial class XamlFilterComboBox : UserControl
    {
        private static readonly DependencyProperty FilterFieldNameProperty = DependencyProperty.Register("FilterFieldName", typeof(string), typeof(XamlFilterComboBox));
        private static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(XamlFilterComboBox));
        private static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(XamlFilterComboBox));

        public XamlFilterComboBox()
        {
            InitializeComponent();

            Binding selectedItemBinding = new Binding("SelectedItem");
            selectedItemBinding.Source = this;
            selectedItemBinding.Mode = BindingMode.TwoWay;
            comboBox.SetBinding(ComboBox.SelectedItemProperty, selectedItemBinding);
        }

        public string FilterFieldName
        {
            get { return (string)GetValue(FilterFieldNameProperty); }
            set { SetValue(FilterFieldNameProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
