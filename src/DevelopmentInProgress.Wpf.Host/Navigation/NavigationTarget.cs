//-----------------------------------------------------------------------
// <copyright file="NavigationTarget.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System;

namespace DevelopmentInProgress.Wpf.Host.Navigation
{
    /// <summary>
    /// Describes the target navigation view.
    /// </summary>
    public class NavigationTarget
    {
        private string navigationHistory;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationTarget"/> class.
        /// </summary>
        /// <param name="target">The target view.</param>
        public NavigationTarget(string target)
        {
            string[] targetSplit = target.Split('^');
            if (targetSplit.Length == 2)
            {
                NavigationId = targetSplit[0];
                Title = targetSplit[1];
                Target = target;
                NavigationHistory = Target;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationTarget"/> class.
        /// </summary>
        /// <param name="navigationId">The navigation id.</param>
        /// <param name="title">The title of the target view.</param>
        public NavigationTarget(string navigationId, string title)
        {
            NavigationId = navigationId;
            Title = title;
            Target = String.Format("{0}^{1}", navigationId, title);
            NavigationHistory = Target;
        }

        /// <summary>
        /// Gets the unique identifier for the target view.
        /// </summary>
        public string NavigationId { get; private set; }

        /// <summary>
        /// Gets the title of the target view.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the target to navigate to.
        /// </summary>
        public string Target { get; private set; }

        /// <summary>
        /// Gets the navigation history.
        /// </summary>
        public string NavigationHistory 
        {
            get { return navigationHistory; } 
            private set { navigationHistory = value; } 
        }

        /// <summary>
        /// Builds and returns an string array of target views.
        /// </summary>
        /// <param name="navigationHistory">The navigation history.</param>
        /// <returns>A string array of target views that make up the navigation history.</returns>
        public static string[] GetNavigationHistory(string navigationHistory)
        {
            if (String.IsNullOrEmpty(navigationHistory))
            {
                navigationHistory = String.Empty;
            }

            return navigationHistory.Split('|');
        }

        /// <summary>
        /// Appends a navigation history to the current navigation target.
        /// </summary>
        /// <param name="navigationHistory">The navigation history.</param>
        public void AppendNavigationHistory(string[] navigationHistory)
        {
            this.navigationHistory = String.Format("{0}|{1}", String.Join("|", navigationHistory), Target);
        }
    }
}
