//-----------------------------------------------------------------------
// <copyright file="NavigationPanel.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevelopmentInProgress.TradeView.Wpf.Controls.Command;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.Navigation
{
    /// <summary>
    /// The navigation panel class.
    /// </summary>
    public class NavigationPanel : Control
    {
        private ICommand selectionChangedCommand;
        private ICommand expanderChangedCommand;

        private readonly static DependencyProperty SelectedNavigationPanelItemProperty = 
            DependencyProperty.Register("SelectedNavigationPanelItem", typeof(NavigationPanelItem), typeof(NavigationPanel));

        private readonly static DependencyProperty NavigationPanelItemsProperty =
            DependencyProperty.Register("NavigationPanelItems", typeof(ObservableCollection<NavigationPanelItem>), typeof(NavigationPanel), 
                new FrameworkPropertyMetadata(new ObservableCollection<NavigationPanelItem>()));

        private readonly static DependencyProperty IsExpandedProperty = 
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(NavigationPanel), 
            new FrameworkPropertyMetadata(true));

        private readonly static RoutedEvent ItemSelectedEvent =
            EventManager.RegisterRoutedEvent("ItemSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavigationPanel));

        /// <summary>
        /// Static constructor for <see cref="NavigationPanel"/> registers dependency properties and events.
        /// </summary>
        static NavigationPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationPanel),
                new FrameworkPropertyMetadata(typeof(NavigationPanel)));
        }

        /// <summary>
        /// Initializes a new instance of the NavigationPanel class.
        /// </summary>
        public NavigationPanel()
        {
            NavigationPanelItems = new ObservableCollection<NavigationPanelItem>();
            selectionChangedCommand = new WpfCommand(OnSelectionChanged);
            expanderChangedCommand = new WpfCommand(OnExpanderChanged);
            IsExpanded = true;
        }
        
        /// <summary>
        /// Uses System.Windows.Interactivity in the Xaml where the 
        /// ListBox.SelectionChanged event triggers the SelectionChangedCommand. 
        /// </summary>
        public ICommand SelectionChangedCommand
        {
            get { return selectionChangedCommand; }
            set { selectionChangedCommand = value; }
        }

        /// <summary>
        /// Uses System.Windows.Interactivity in the Xaml where the
        /// Image.MouseDown event triggers the ExpanderChangedCommand.
        /// </summary>
        public ICommand ExpanderChangedCommand
        {
            get { return expanderChangedCommand; }
            set { expanderChangedCommand = value; }
        }

        /// <summary>
        /// Gets or sets the selected <see cref="NavigationPanelItem"/>.
        /// </summary>
        public NavigationPanelItem SelectedNavigationPanelItem
        {
            get { return (NavigationPanelItem)GetValue(SelectedNavigationPanelItemProperty); }
            set { SetValue(SelectedNavigationPanelItemProperty, value); }
        }


        /// <summary>
        /// Gets or sets a list of <see cref="NavigationPanelItem"/>'s.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Required for binding in Xaml")]
        public ObservableCollection<NavigationPanelItem> NavigationPanelItems
        {
            get { return (ObservableCollection<NavigationPanelItem>)GetValue(NavigationPanelItemsProperty); }
            set { SetValue(NavigationPanelItemsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the navigation panel list is expanded or collapsed.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="NavigationPanelItem"/> is selected.
        /// </summary>
        public event RoutedEventHandler ItemSelected
        {
            add { AddHandler(ItemSelectedEvent, value); }
            remove { RemoveHandler(ItemSelectedEvent, value); }
        }

        /// <summary>
        /// Raises the ItemSelectedEvent passing in the selected NavigationPanelItem.
        /// OnSelectionChanged handles the ListBox.SelectionChanged event which  
        /// triggers the SelectionChangedCommand using System.Windows.Interactivity.
        /// </summary>
        /// <param name="arg">The selected NavigationPanelItem.</param>
        private void OnSelectionChanged(object arg)
        {
            if (arg == null)
            {
                return;
            }

            var navigationPanelItem = arg as NavigationPanelItem;
            var args = new RoutedEventArgs(ItemSelectedEvent, navigationPanelItem);
            RaiseEvent(args);
        }

        /// <summary>
        /// Toggles the navigation panel list expanded / un-expanded.
        /// </summary>
        /// <param name="arg">Null</param>
        private void OnExpanderChanged(object arg)
        {

            if (arg == null)
            {
                return;
            }

            SelectedNavigationPanelItem = arg as NavigationPanelItem;
            IsExpanded = !IsExpanded;
        }
    }
}
