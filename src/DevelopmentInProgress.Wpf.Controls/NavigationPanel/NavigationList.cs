//-----------------------------------------------------------------------
// <copyright file="NavigationList.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.Wpf.Controls.NavigationPanel
{
    /// <summary>
    /// The NavigationList class.
    /// </summary>
    public class NavigationList : Control
    {
        private readonly static DependencyProperty NavigationListNameProperty;
        private readonly static DependencyProperty NavigationListItemsProperty;

        /// <summary>
        /// Static constructor for the <see cref="NavigationList"/> class for registering dependency properties and events.
        /// </summary>
        static NavigationList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationList), new FrameworkPropertyMetadata(typeof(NavigationList)));

            NavigationListNameProperty = DependencyProperty.Register("NavigationListName", typeof(string), typeof(NavigationList));
            NavigationListItemsProperty = DependencyProperty.Register("NavigationListItems", typeof(List<NavigationListItem>), 
                typeof(NavigationList), new FrameworkPropertyMetadata(new List<NavigationListItem>()));
        }

        /// <summary>
        /// Initializes a new instance of the NavigationList class.
        /// </summary>
        public NavigationList()
        {
            NavigationListItems = new List<NavigationListItem>();
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
        public List<NavigationListItem> NavigationListItems
        {
            get { return (List<NavigationListItem>)GetValue(NavigationListItemsProperty); }
            set { SetValue(NavigationListItemsProperty, value); }
        }
    }
}