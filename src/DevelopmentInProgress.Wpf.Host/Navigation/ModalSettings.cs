//-----------------------------------------------------------------------
// <copyright file="ModalSettings.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.Host.Navigation
{
    /// <summary>
    /// Contains settings used for displaying modal dialogs windows.
    /// </summary>
    public class ModalSettings : ViewSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModalSettings"/> class.
        /// </summary>
        public ModalSettings()
        {
            Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the name of the view model to instantiate using unity.
        /// </summary>
        public string ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the height of the modal window.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the width of the modal window.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets the parameters to be passed to the modal window.
        /// </summary>
        public Dictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// gets or sets the result of the modal window returned to the calling code.
        /// </summary>
        public bool? Result { get; set; }

        /// <summary>
        /// Gets or sets the resulting output of the modal window to be consumed by the calling code.
        /// </summary>
        public object Output { get; set; }
    }
}
