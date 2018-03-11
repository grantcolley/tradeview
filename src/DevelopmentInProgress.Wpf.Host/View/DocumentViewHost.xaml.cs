//-----------------------------------------------------------------------
// <copyright file="DocumentViewHost.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Windows.Controls;
using DevelopmentInProgress.Wpf.Host.RegionAdapters;

namespace DevelopmentInProgress.Wpf.Host.View
{
    /// <summary>
    /// The host for the document view set as the content for the
    /// regions host control in <see cref="DockingManagerBehavior"/>. 
    /// The <see cref="DocumentViewHost"/> provides common functionality
    /// such as navigation history, refresh and messaging.
    /// </summary>
    public partial class DocumentViewHost : UserControl
    {
        public DocumentViewHost(DocumentViewBase documentViewBase)
        {
            InitializeComponent();

            MainContent.Content = documentViewBase;
            DataContext = documentViewBase.DataContext;
            ModuleName = documentViewBase.ModuleName;
        }

        /// <summary>
        /// Gets or sets the module name.
        /// </summary>
        public string ModuleName { get; private set; }

        /// <summary>
        /// Gets an instance of the <see cref="DocumentViewBase"/>.
        /// </summary>
        public DocumentViewBase View
        {
            get { return MainContent.Content as DocumentViewBase; }
        }
    }
}
