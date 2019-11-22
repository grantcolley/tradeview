//-----------------------------------------------------------------------
// <copyright file="XamlFilterBox.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2013. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.FilterBox
{
    /// <summary>
    /// A filter control. Bind a list to it and filter by the text typed into the text box.
    /// </summary>
    public class XamlFilterBox : Control
    {
        private static readonly DependencyProperty FilterTextProperty;
        private static readonly DependencyProperty FilterFieldNameProperty;
        private static readonly DependencyProperty VisibilityFieldNameProperty;
        private static readonly DependencyProperty SupportsDeepTraversalProperty;
        private static readonly DependencyProperty ItemsSourceProperty;

        static XamlFilterBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XamlFilterBox), new FrameworkPropertyMetadata(typeof(XamlFilterBox)));

            FilterTextProperty = DependencyProperty.Register("FilterText", typeof(string), typeof(XamlFilterBox));

            FilterFieldNameProperty = DependencyProperty.Register("FilterFieldName", typeof(string), typeof(XamlFilterBox));

            VisibilityFieldNameProperty = DependencyProperty.Register("VisibilityFieldName", typeof(string), typeof(XamlFilterBox));

            SupportsDeepTraversalProperty = DependencyProperty.Register("SupportsDeepTraversal", typeof(bool), typeof(XamlFilterBox), new FrameworkPropertyMetadata(true));

            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(XamlFilterBox));
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
        /// Gets or sets a flag indicating whether deep nested tranersal is supported.
        /// Deep nested traversal i.e. travering nested lists for the visibility field 
        /// name may unecessarily impact performance when searching is only required 
        /// at the root level. Default is true.
        /// </summary>
        public bool SupportsDeepTraversal
        {
            get { return (bool)GetValue(SupportsDeepTraversalProperty); }
            set { SetValue(SupportsDeepTraversalProperty, value); }
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
