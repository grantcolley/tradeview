//-----------------------------------------------------------------------
// <copyright file="FilterTree.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2015. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.FilterTree
{
    /// <summary>
    /// The <see cref="FilterTree"/> class provides the code behind the resource dictionary. 
    /// </summary>
    partial class FilterTree
    {
        #region Filtering operations

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            var xamlFilterTree = textBox.Tag as XamlFilterTree;
            if (xamlFilterTree == null)
            {
                return;
            }

            var items = xamlFilterTree.ItemsSource;
            Contains(items, textBox.Text);
        }

        private bool Contains(IEnumerable items, string text)
        {
            bool result = false;
            foreach (var item in items)
            {
                var innerResult = false;
                var properties = item.GetType().GetProperties();
                foreach (var property in properties)
                {
                    if (
                        property.PropertyType.GetInterfaces()
                            .Any(
                                i =>
                                    i.IsGenericType &&
                                    i.GetGenericTypeDefinition().Name.Equals(typeof (IEnumerable<>).Name)))
                    {
                        foreach (var itemType in property.PropertyType.GetGenericArguments())
                        {
                            var textPropertyInfo = itemType.GetProperty("Text");
                            var visiblePropertyInfo = itemType.GetProperty("IsVisible");

                            if (textPropertyInfo != null
                                && visiblePropertyInfo != null)
                            {
                                if (Contains((IEnumerable) property.GetValue(item, null), text))
                                {
                                    innerResult = true;
                                }
                            }
                        }
                    }
                }

                if (Contains(item, text, innerResult))
                {
                    result = true;
                }
            }

            return result;
        }

        private bool Contains<T>(T t, string text, bool hasVisibleChild)
        {
            var textPropertyInfo = t.GetType().GetProperty("Text");
            var visiblePropertyInfo = t.GetType().GetProperty("IsVisible");

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

        #endregion

        #region Selected item operations

        private void OnSelectItemDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            OnSelectItem(sender);
            e.Handled = true;
        }

        private void OnSelectItemKeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
            {
                return;
            }

            OnSelectItem(sender);
            e.Handled = true;
        }

        private void OnSelectItem<T>(T sender)
        {
            var item = sender as TreeViewItem;
            if (item == null)
            {
                return;
            }

            if (item.IsSelected)
            {
                var xamlFilterTree = item.Tag as XamlFilterTree;
                if (xamlFilterTree == null
                    || xamlFilterTree.SelectItemCommand == null)
                {
                    return;
                }

                xamlFilterTree.SelectItemCommand.Execute(item.Header);
            }
        }

        #endregion

        #region Drag and drop operations

        private Point startPoint;
        private TreeViewItem dragItem;

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item == null
                || !item.IsSelected)
            {
                return;
            }

            var xamlFilterTree = item.Tag as XamlFilterTree;
            if (xamlFilterTree == null
                || !xamlFilterTree.IsEditable)
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                dragItem = item;
                startPoint = e.GetPosition(item);
                DragDrop.DoDragDrop(item, new DataObject(dragItem), DragDropEffects.Move);
            }
        }

        private void DragOverHandler(object sender, DragEventArgs e)
        {
            var currentUiElement = e.OriginalSource as UIElement;
            if (currentUiElement == null)
            {
                return;
            }

            Point currentPosition = e.GetPosition(currentUiElement);

            if ((Math.Abs(currentPosition.X - startPoint.X) > 10)
                || (Math.Abs(currentPosition.Y - startPoint.Y) > 10))
            {
                TreeViewItem dropItem = GetNearestTreeViewItem(currentUiElement);
                if (dropItem != null)
                {
                    if (dragItem.ToolTip.ToString().Equals(dropItem.ToolTip.ToString()))
                    {
                        e.Effects = DragDropEffects.None;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                }
            }

            e.Handled = true;
        }

        private TreeViewItem GetNearestTreeViewItem(UIElement uiElement)
        {
            TreeViewItem treeViewItem = uiElement as TreeViewItem;
            while (treeViewItem == null
                && uiElement != null)
            {
                uiElement = VisualTreeHelper.GetParent(uiElement) as UIElement;
                treeViewItem = uiElement as TreeViewItem;
            }

            return treeViewItem;
        }

        private void DropHandler(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            TreeViewItem targetItem = GetNearestTreeViewItem(e.OriginalSource as UIElement);
            if (targetItem != null && dragItem != null)
            {
                var xamlFilterTree = dragItem.Tag as XamlFilterTree;
                if (xamlFilterTree != null
                    && xamlFilterTree.DragDropCommand != null)
                {
                    xamlFilterTree.DragDropCommand.Execute(new FilterTreeDragDropArgs(dragItem.Header, targetItem.Header));
                }

                dragItem = null;
            }
        }

        #endregion
    }
}
