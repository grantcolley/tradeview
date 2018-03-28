//-----------------------------------------------------------------------
// <copyright file="XamlFilterComboBoxRx.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2013. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.Wpf.Controls.FilterComboBoxRx
{
    /// <summary>
    /// A filter control. Bind a list to it and filter by the text typed into the text box.
    /// </summary>
    public class XamlFilterComboBoxRx : Control
    {
        private static readonly DependencyProperty FilterTextProperty;
        private static readonly DependencyProperty FilterFieldNameProperty;
        private static readonly DependencyProperty VisibilityFieldNameProperty;
        private static readonly DependencyProperty ItemsSourceProperty;

        static XamlFilterComboBoxRx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XamlFilterComboBoxRx), new FrameworkPropertyMetadata(typeof(XamlFilterComboBoxRx)));

            FilterTextProperty = DependencyProperty.Register("FilterText", typeof(string), typeof(XamlFilterComboBoxRx));

            FilterFieldNameProperty = DependencyProperty.Register("FilterFieldName", typeof(string), typeof(XamlFilterComboBoxRx));

            VisibilityFieldNameProperty = DependencyProperty.Register("VisibilityFieldName", typeof(string), typeof(XamlFilterComboBoxRx));
            
            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(XamlFilterComboBoxRx));
        }

        /// <summary>
        /// Gets or sets the filter text.
        /// </summary>
        public string FilterText
        {
            get { return GetValue(FilterTextProperty)?.ToString(); }
            set { SetValue(FilterTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the filter field name of the target object.
        /// </summary>
        public string FilterFieldName
        {
            get { return GetValue(FilterFieldNameProperty)?.ToString(); }
            set { SetValue(FilterFieldNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the visibility field name of the target object.
        /// </summary>
        public string VisibilityFieldName
        {
            get { return GetValue(VisibilityFieldNameProperty)?.ToString(); }
            set { SetValue(VisibilityFieldNameProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
    }
}
