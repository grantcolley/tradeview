//-----------------------------------------------------------------------
// <copyright file="ViewSettings.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation
{
    /// <summary>
    /// Abstract class providing base view settings.
    /// </summary>
    public abstract class ViewSettings
    {
        /// <summary>
        /// Gets or sets the view title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the target view.
        /// </summary>
        public string View { get; set; }
    }
}
