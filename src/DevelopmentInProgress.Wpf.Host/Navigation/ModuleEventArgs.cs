//-----------------------------------------------------------------------
// <copyright file="ModuleEventArgs.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System;

namespace DevelopmentInProgress.Wpf.Host.Navigation
{
    /// <summary>
    /// Passing module data with module events.
    /// </summary>
    public class ModuleEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleEventArgs"/> class.
        /// </summary>
        /// <param name="moduleName">The module name.</param>
        public ModuleEventArgs(string moduleName)
        {
            ModuleName = moduleName;
        }

        /// <summary>
        /// Gets the module name.
        /// </summary>
        public string ModuleName {get;private set;}
    }
}
