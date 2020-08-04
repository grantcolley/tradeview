//-----------------------------------------------------------------------
// <copyright file="NavigationList.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.NavigationPanel
{
    /// <summary>
    /// The NavigationList class.
    /// </summary>
    public class NavigationList : Control
    {
        private readonly static DependencyProperty NavigationListNameProperty = 
            DependencyProperty.Register("NavigationListName", typeof(string), typeof(NavigationList));

        private readonly static DependencyProperty NavigationListItemsProperty =
            DependencyProperty.Register("NavigationListItems", typeof(ObservableCollection<NavigationListItem>), typeof(NavigationList), 
                new FrameworkPropertyMetadata(new ObservableCollection<NavigationListItem>()));

        /// <summary>
        /// Static constructor for the <see cref="NavigationList"/> class for registering dependency properties and events.
        /// </summary>
        static NavigationList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationList), new FrameworkPropertyMetadata(typeof(NavigationList)));
        }

        /// <summary>
        /// Initializes a new instance of the NavigationList class.
        /// </summary>
        public NavigationList()
        {
            NavigationListItems = new ObservableCollection<NavigationListItem>();
        }

        /// <summary>
        /// Gets or sets the navigation list name.
        /// </summary>
        public string NavigationListName
        {
            get { return GetValue(NavigationListNameProperty).ToString(); }
            set { SetValue(NavigationListNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the list of <see cref="NavigationListItem"/>'s.
        /// </summary>
        public ObservableCollection<NavigationListItem> NavigationListItems
        {
            get { return (ObservableCollection<NavigationListItem>)GetValue(NavigationListItemsProperty); }
            set { SetValue(NavigationListItemsProperty, value); }
        }
    }
}