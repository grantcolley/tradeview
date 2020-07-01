//-----------------------------------------------------------------------
// <copyright file="ModuleGroupItem.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation
{
    /// <summary>
    /// Module items contain details of the target view to instantiate.
    /// </summary>
    public class ModuleGroupItem
    {
        /// <summary>
        /// Gets or sets the display name of the item.
        /// </summary>
        public string ModuleGroupItemName { get; set; }

        /// <summary>
        /// Gets or sets the path to the image to be displayed for the item.
        /// </summary>
        public string ModuleGroupItemImagePath { get; set; }

        /// <summary>
        /// Gets or sets the title of the target view.
        /// </summary>
        public string TargetViewTitle { get; set; }

        /// <summary>
        /// Gets or sets the name of the target view to be instantiated.
        /// </summary>
        public string TargetView { get; set; }
    }
}
