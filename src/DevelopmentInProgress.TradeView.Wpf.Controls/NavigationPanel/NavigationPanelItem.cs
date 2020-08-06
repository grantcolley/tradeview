//-----------------------------------------------------------------------
// <copyright file="NavigationPanelItem.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.NavigationPanel
{
    /// <summary>
    /// A navigation panel item. 
    /// </summary>
    public class NavigationPanelItem : Control, ICommandSource
    {
        private readonly static DependencyProperty NavigationPanelItemNameProperty = 
            DependencyProperty.Register("NavigationPanelItemName", typeof(string), typeof(NavigationPanelItem));

        private readonly static DependencyProperty ImageLocationProperty =
            DependencyProperty.Register("ImageLocation", typeof(string), typeof(NavigationPanelItem));

        private readonly static DependencyProperty NavigationListProperty =
            DependencyProperty.Register("NavigationList", typeof(List<NavigationList>), typeof(NavigationList), 
                new FrameworkPropertyMetadata(new List<NavigationList>()));

        private readonly static DependencyProperty IsSelectedProperty = 
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(NavigationPanelItem));

        private readonly static DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(NavigationPanelItem));

        private readonly static DependencyProperty CommandParameterProperty = 
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(NavigationPanelItem));

        private readonly static DependencyProperty CommandTargetProperty = 
            DependencyProperty.Register("CommandTarget", typeof(UIElement), typeof(NavigationPanelItem));

        private readonly static RoutedEvent ItemClickedEvent = 
            EventManager.RegisterRoutedEvent("ItemClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavigationPanelItem));

        /// <summary>
        /// Static constructor for the <see cref="NavigationPanelItem"/> 
        /// class registering dependency properties and events.
        /// </summary>
        static NavigationPanelItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationPanelItem), new FrameworkPropertyMetadata(typeof(NavigationPanelItem)));
        }

        /// <summary>
        /// Initializes a new instance of the NavigationPanelItem class. 
        /// </summary>
        public NavigationPanelItem()
        {
            NavigationList = new List<NavigationList>();
        }

        /// <summary>
        /// Gets or sets the <see cref="NavigationPanelItem"/> name.
        /// </summary>
        public string NavigationPanelItemName
        {
            get { return GetValue(NavigationPanelItemNameProperty).ToString(); }
            set { SetValue(NavigationPanelItemNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the image location.
        /// </summary>
        public string ImageLocation
        {
            get { return (string)GetValue(ImageLocationProperty); }
            set { SetValue(ImageLocationProperty, value); }
        }


        /// <summary>
        /// Gets or sets the list of <see cref="NavigationList"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Required for binding in Xaml")]
        public List<NavigationList> NavigationList
        {
            get { return (List<NavigationList>)GetValue(NavigationListProperty); }
            set { SetValue(NavigationListProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="NavigationPanelItem"/> is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command target.
        /// </summary>
        public IInputElement CommandTarget
        {
            get { return (UIElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the item clicked event.
        /// </summary>
        public event RoutedEventHandler ItemClicked
        {
            add { AddHandler(ItemClickedEvent, value); }
            remove { RemoveHandler(ItemClickedEvent, value); }
        }

        /// <summary>
        /// Overrides the mouse's left button up preview.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Raises the mouse's left button up preview event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            OnItemClicked();
            e.Handled = true;
        }

        /// <summary>
        /// Raises the item clicked event.
        /// </summary>
        private void OnItemClicked()
        {
            var args = new RoutedEventArgs(ItemClickedEvent, this);
            RaiseEvent(args);
            if (Command != null)
            {
                Command.Execute(CommandParameter);
            }
        }
    }
}
