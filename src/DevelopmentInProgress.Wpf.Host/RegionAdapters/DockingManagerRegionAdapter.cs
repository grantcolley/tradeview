//-----------------------------------------------------------------------
// <copyright file="DockingManagerRegionAdapter.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using Microsoft.Practices.Prism.Regions;
using Xceed.Wpf.AvalonDock;

namespace DevelopmentInProgress.Wpf.Host.RegionAdapters
{
    /// <summary>
    /// The region adapter for the docking manager.
    /// </summary>
    public class DockingManagerRegionAdapter : RegionAdapterBase<DockingManager>
    {
        public DockingManagerRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        /// <summary>
        /// Implementation required by <see cref="RegionAdapterBase"/>.
        /// </summary>
        /// <returns>Returns the <see cref="IRegion"/></returns>
        protected override IRegion CreateRegion()
        {
            return new Region();
        }

        /// <summary>
        /// Implementation required by <see cref="RegionAdapterBase"/>.
        /// </summary>
        /// <param name="region">The <see cref="IRegion"/></param>
        /// <param name="regionTarget">The <see cref="DockingManager"/></param>
        protected override void Adapt(IRegion region, DockingManager regionTarget)
        {
        }

        /// <summary>
        /// Attaches behavioural objects to the regions behaviours collection.
        /// </summary>
        /// <param name="region">The <see cref="IRegion"/></param>
        /// <param name="regionTarget">The <see cref="DockingManager"/></param>
        protected override void AttachBehaviors(IRegion region, DockingManager regionTarget)
        {
            if (region == null)
            {
                throw new System.ArgumentNullException("region");
            }

            region.Behaviors.Add(DockingManagerBehavior.BehaviorKey,
                new DockingManagerBehavior()
            {
                HostControl = regionTarget
            });

            base.AttachBehaviors(region, regionTarget);
        }
    }
}