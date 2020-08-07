//-----------------------------------------------------------------------
// <copyright file="NavigationPanelResources.cs" company="Development In Progress Ltd">
//     Copyright © 2015. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Windows.Controls;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.Navigation
{
    /// <summary>
    /// The <see cref="NavigationPanelResources"/> class provides the code behind the resource dictionary. 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1010:Generic interface should also be implemented")]
    partial class NavigationPanelResources
    {
        private void ExpanderImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Image image)
                || image.Tag == null)
            {
                return;
            }

            if (!(image.Tag is NavigationPanel navigationPanel))
            {
                return;
            }

            navigationPanel.ExpanderChangedCommand.Execute(navigationPanel.SelectedNavigationPanelItem);
        }

        private void NavigationPanelItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ListBox listBox)
                || listBox.Tag == null)
            {
                return;
            }

            if (!(listBox.Tag is NavigationPanel navigationPanel))
            {
                return;
            }

            navigationPanel.SelectionChangedCommand.Execute(navigationPanel.SelectedNavigationPanelItem);
        }
    }
}
