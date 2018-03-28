//-----------------------------------------------------------------------
// <copyright file="FilterBox.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2015. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DevelopmentInProgress.Wpf.Controls.FilterComboBoxRx
{
    /// <summary>
    /// The <see cref="FilterComboBoxRx"/> class provides the code behind the resource dictionary. 
    /// </summary>
    partial class FilterComboBoxRx
    {
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            var xamlFilterBox = textBox.Tag as XamlFilterComboBoxRx;
            if (xamlFilterBox == null)
            {
                return;
            }

            var items = xamlFilterBox.ItemsSource;
            if (items == null)
            {
                return;
            }
            
            var filterFieldName = xamlFilterBox.FilterFieldName;
            if(string.IsNullOrWhiteSpace(filterFieldName))
            {
                return;
            }

            var visibilityFieldName = xamlFilterBox.VisibilityFieldName;
            if(string.IsNullOrWhiteSpace(visibilityFieldName))
            {
                return;
            }

            Contains(items, textBox.Text, filterFieldName, visibilityFieldName, false);
        }

        private bool Contains(IEnumerable items, string text, string filterFieldName, string visibilityFieldName, bool supportsDeepTraversal)
        {
            bool result = false;
            foreach (var item in items)
            {
                var innerResult = false;

                if (supportsDeepTraversal)
                {
                    var properties = item.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        if (
                            property.PropertyType.GetInterfaces()
                                .Any(
                                    i =>
                                        i.IsGenericType &&
                                        i.GetGenericTypeDefinition().Name.Equals(typeof(IEnumerable<>).Name)))
                        {
                            foreach (var itemType in property.PropertyType.GetGenericArguments())
                            {
                                var textPropertyInfo = itemType.GetProperty(filterFieldName);
                                var visiblePropertyInfo = itemType.GetProperty(visibilityFieldName);

                                if (textPropertyInfo != null
                                    && visiblePropertyInfo != null)
                                {
                                    if (Contains((IEnumerable)property.GetValue(item, null), text, filterFieldName, visibilityFieldName, supportsDeepTraversal))
                                    {
                                        innerResult = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (Contains(item, text, innerResult, filterFieldName, visibilityFieldName))
                {
                    result = true;
                }
            }

            return result;
        }

        private bool Contains<T>(T t, string text, bool hasVisibleChild, string filterFieldName, string visibilityFieldName)
        {
            var textPropertyInfo = t.GetType().GetProperty(filterFieldName);
            var visiblePropertyInfo = t.GetType().GetProperty(visibilityFieldName);

            if (textPropertyInfo != null
                && visiblePropertyInfo != null)
            {
                if (string.IsNullOrEmpty(text)
                    || hasVisibleChild)
                {
                    visiblePropertyInfo.SetValue(t, true, null);
                    return true;
                }

                var val = textPropertyInfo.GetValue(t, null);
                if (val != null
                    && val.ToString().ToLower().Contains(text.ToLower()))
                {
                    visiblePropertyInfo.SetValue(t, true, null);
                    return true;
                }

                visiblePropertyInfo.SetValue(t, false, null);
            }

            return false;
        }
    }
}
