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
        private static readonly DependencyProperty FilterFieldNameProperty;
        private static readonly DependencyProperty ItemsSourceProperty;
        private static readonly DependencyProperty FilterItemsSourceProperty;
        private static readonly DependencyProperty SelectedItemProperty;

        static XamlFilterComboBox()
        {
            FilterFieldNameProperty = DependencyProperty.Register("FilterFieldName", typeof(string), typeof(XamlFilterComboBox));
            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(XamlFilterComboBox), new PropertyMetadata(default(IEnumerable), popertyChangedCallback));
            FilterItemsSourceProperty = DependencyProperty.Register("FilterItemsSource", typeof(IEnumerable), typeof(XamlFilterComboBox));
            SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(XamlFilterComboBox));
        }

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

        public IEnumerable FilterItemsSource
        {
            get { return (IEnumerable)GetValue(FilterItemsSourceProperty); }
            set { SetValue(FilterItemsSourceProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        private static void popertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(XamlFilterComboBox.FilterItemsSourceProperty, e.NewValue);
        }
    }
}
