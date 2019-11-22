//-----------------------------------------------------------------------
// <copyright file="ModuleSettings.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Navigation
{
    /// <summary>
    /// Contains details of a module consumed by the module manager.
    /// </summary>
    public class ModuleSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleSettings"/> class.
        /// </summary>
        public ModuleSettings()
        {
            ModuleGroups = new List<ModuleGroup>();
        }

        /// <summary>
        /// Gets or sets the module name.
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the path to the module image to be displayed.
        /// </summary>
        public string ModuleImagePath { get; set; }

        /// <summary>
        /// Gets a list of <see cref="ModuleGroup"/> which group module items.
        /// </summary>
        public List<ModuleGroup> ModuleGroups { get; private set; }
    }
}
