//-----------------------------------------------------------------------
// <copyright file="ModuleGroup.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.Host.Navigation
{
    /// <summary>
    /// Groups or categorizes module items.
    /// </summary>
    public class ModuleGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleGroup"/> class.
        /// </summary>
        public ModuleGroup()
        {
            ModuleGroupItems = new List<ModuleGroupItem>();
        }

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        public string ModuleGroupName { get; set; }

        /// <summary>
        /// Gets or sets the path to the image to be displayed for the module group.
        /// </summary>
        public string ModuleGroupImagePath { get; set; }

        /// <summary>
        /// Gets a list of <see cref="ModuleGroupItem"/>.
        /// </summary>
        public List<ModuleGroupItem> ModuleGroupItems { get; private set; }
    }
}
