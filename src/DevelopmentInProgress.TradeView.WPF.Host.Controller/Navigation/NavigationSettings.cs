//-----------------------------------------------------------------------
// <copyright file="NavigationSettings.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation
{
    /// <summary>
    /// Contains navigation details for the target view.
    /// </summary>
    public class NavigationSettings : ViewSettings, ICloneable
    {
        private object data = new object();

        /// <summary>
        /// Gets or sets the unique identifier for the navigation view.
        /// </summary>
        public string NavigationId { get; set; }

        /// <summary>
        /// Gets or sets the breadcrumb path from the originating document view to
        /// the current view. Enables navigation back to the originating document.
        /// </summary>
        public string NavigationHistory { get; set; }

        /// <summary>
        /// Builds a partial uri which excludes the navigation id,
        /// to assist identify the object. Note, this is not 
        /// intended to be a unique identifier.
        /// </summary>
        public string PartialQuery { get; set; }

        /// <summary>
        /// Gets or sets the uri of the view to navigate to.
        /// </summary>
        public string ViewQuery { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DocumentViewBase"/>.
        /// </summary>
        public DocumentViewBase DocumentView { get; set; }

        /// <summary>
        /// Gets or sets the parameter passed to the view model of the target view.
        /// </summary>
        public object Data
        {
            get { return data; }
            set 
            {
                if (value != null)
                {
                    data = value;
                }
                else
                {
                    data = new object();
                }
            }
        }

        /// <summary>
        /// Makes a memberwise clone of the <see cref="NavigationSettings"/> object.
        /// </summary>
        /// <returns>A clone of the <see cref="NavigationSettings"/> object.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
