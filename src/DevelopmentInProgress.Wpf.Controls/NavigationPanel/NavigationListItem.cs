//-----------------------------------------------------------------------
// <copyright file="NavigationListItem.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevelopmentInProgress.Wpf.Controls.NavigationPanel
{
    /// <summary>
    /// The NavigationListItem class.
    /// </summary>
    public class NavigationListItem : Control, ICommandSource
    {
        private readonly static DependencyProperty ItemNameProperty;
        private readonly static DependencyProperty ImageLocationProperty;
        private readonly static DependencyProperty CommandProperty;
        private readonly static DependencyProperty CommandParameterProperty;
        private readonly static DependencyProperty CommandTargetProperty;
        private readonly static RoutedEvent ItemClickedEvent;

        /// <summary>
        /// Static constructor for the <see cref="NavigationListItem"/> class for registering dependency properties and events.
        /// </summary>
        static NavigationListItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(NavigationListItem), new FrameworkPropertyMetadata(typeof(NavigationListItem)));

            ItemNameProperty = DependencyProperty.Register(
                "ItemName", typeof(string), typeof(NavigationListItem));
            
            ImageLocationProperty = DependencyProperty.Register(
                "ImageLocation", typeof(string), typeof(NavigationListItem));

            CommandProperty = DependencyProperty.Register(
                "Command", typeof(ICommand), typeof(NavigationListItem));

            CommandParameterProperty = DependencyProperty.Register(
                "CommandParameter", typeof(object), typeof(NavigationListItem));

            CommandTargetProperty = DependencyProperty.Register(
                "CommandTarget", typeof(UIElement), typeof(NavigationListItem));

            ItemClickedEvent = EventManager.RegisterRoutedEvent(
                "ItemClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavigationListItem));
        }

        /// <summary>
        /// Gets or sets the item name.
        /// </summary>
        public string ItemName
        {
            get { return GetValue(ItemNameProperty).ToString(); }
            set { SetValue(ItemNameProperty, value); }
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