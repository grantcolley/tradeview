//-----------------------------------------------------------------------
// <copyright file="XamlFilterTree.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2013. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.FilterTree
{
    /// <summary>
    /// The XamlFilterTree control displays hierarchical data that can be filtered
    /// dynamically. It also provides commands for adding and removing new items 
    /// to the hierarchy via context menues and facilitates drag and drop.
    /// </summary>
    public class XamlFilterTree : Control
    {
        private static readonly DependencyProperty FilterTextProperty;
        private static readonly DependencyProperty ItemsSourceProperty;
        private static readonly DependencyProperty AddItemCommandProperty;
        private static readonly DependencyProperty RemoveItemCommandProperty;
        private static readonly DependencyProperty SelectItemCommandProperty;
        private static readonly DependencyProperty DragDropCommandProperty;
        private static readonly DependencyProperty IsEditableProperty;

        /// <summary>
        /// Static constructor for registering dependency properties.
        /// </summary>
        static XamlFilterTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (XamlFilterTree),
                new FrameworkPropertyMetadata(typeof (XamlFilterTree)));
            
            FilterTextProperty = DependencyProperty.Register("FilterText", typeof (string), typeof (XamlFilterTree));

            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
                typeof(XamlFilterTree));

            AddItemCommandProperty = DependencyProperty.Register("AddItemCommand", typeof (ICommand),
                typeof (XamlFilterTree));

            RemoveItemCommandProperty = DependencyProperty.Register("RemoveItemCommand", typeof(ICommand),
                typeof (XamlFilterTree));

            SelectItemCommandProperty = DependencyProperty.Register("SelectItemCommand", typeof (ICommand),
                typeof (XamlFilterTree));

            DragDropCommandProperty = DependencyProperty.Register("DragDropCommand", typeof(ICommand),
                typeof (XamlFilterTree));

            IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool),
                typeof (XamlFilterTree), new PropertyMetadata(true));
        }

        /// <summary>
        /// Gets or sets the filter text.
        /// </summary>
        public string FilterText
        {
            get { return GetValue(FilterTextProperty).ToString(); }
            set { SetValue(FilterTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the hierarchical items source.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command to add items to the hierarchical list.
        /// </summary>
        public ICommand AddItemCommand
        {
            get { return (ICommand)GetValue(AddItemCommandProperty); }
            set { SetValue(AddItemCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command to remove items from the hierarchical list.
        /// </summary>
        public ICommand RemoveItemCommand
        {
            get { return (ICommand)GetValue(RemoveItemCommandProperty); }
            set { SetValue(RemoveItemCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected item command which is executed when pressing
        /// the return key on the selected item or double clicking it with the mouse.
        /// </summary>
        public ICommand SelectItemCommand
        {
            get { return (ICommand)GetValue(SelectItemCommandProperty); }
            set { SetValue(SelectItemCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the drag drop command which is executed when an item is
        /// dragged and dropped onto a target item in the <see cref="XamlFilterTree"/>.
        /// </summary>
        public ICommand DragDropCommand
        {
            get { return (ICommand)GetValue(DragDropCommandProperty); }
            set { SetValue(DragDropCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating wwhether the XamlFilterTree is editable or not.
        /// The XamlFilterTree is editable by default.
        /// </summary>
        public bool IsEditable
        {
            get { return (bool) GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
    }
}
