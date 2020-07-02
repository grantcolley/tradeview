//-----------------------------------------------------------------------
// <copyright file="DockingManagerBehavior.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;
using Prism.Regions;
using Prism.Regions.Behaviors;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Controller.RegionAdapters
{
    /// <summary>
    /// Encapsulates behaviours related to the docking manager, keeping
    /// the documents source in sync with the active views of the <see cref="IRegion"/>. 
    /// </summary>
    public class DockingManagerBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        private DockingManager dockingManager;

        /// <summary>
        /// The target control for the <see cref="IRegion"/>.
        /// </summary>
        public DependencyObject HostControl
        {
            get
            {
                return dockingManager;
            }

            set
            {
                dockingManager = value as DockingManager;
            }
        }

        /// <summary>
        /// The DockingManagerBehavior's key used to 
        /// identify it in the region's behavior collection.
        /// </summary>
        public static readonly string BehaviorKey = "DockingManagerBehavior";

        /// <summary>
        /// Register to handle region events.
        /// </summary>
        protected override void OnAttach()
        {
            dockingManager.ActiveContentChanged += DockingManagerActiveContentChanged;
            Region.ActiveViews.CollectionChanged += ActiveViewsCollectionChanged;
            Region.Views.CollectionChanged += ViewsCollectionChanged;
            ModulesNavigationView.ModuleSelected += ModuleSelected;
        }

        /// <summary>
        /// Handles the changing of the active content.
        /// Notify the active content that it is now 'active'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DockingManagerActiveContentChanged(object sender, EventArgs e)
        {
            var documentViewHost = dockingManager.ActiveContent as DocumentViewHost;
            documentViewHost?.View?.OnActiveChanged(true);
        }

        /// <summary>
        /// Handles changes to the <see cref="IRegion"/> views collection, adding  
        /// or removing items to the <see cref="HostControl"/> document source.
        /// </summary>
        /// <param name="sender">The <see cref="IRegion"/> views collection.</param>
        /// <param name="e">Event arguments.</param>
        private void ViewsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object newItem in e.NewItems)
                {
                    var documentViewBase = newItem as DocumentViewBase;
                    if (documentViewBase != null)
                    {
                        // Get the LayoutDocumentPane from the 
                        // LayoutDocumentPaneControl and add the 
                        // new document to it.
                        var layoutDocumentPaneControl
                            = dockingManager
                            .FindVisualChildren<LayoutDocumentPaneControl>()
                            .First();
                        var layoutDocumentPane = (LayoutDocumentPane)layoutDocumentPaneControl.Model;
                        
                        // Create an AvalonDock LayoutAnchorable object and set it as the HostControl of the 
                        // documentViewBase. Through the HostControl property of the DocumentViewBase we are
                        // able to access properties on the AvalonDock LayoutAnchorable control such as IsActive.
                        var layoutAnchorable = new LayoutAnchorable();
                        layoutAnchorable.Closed += LayoutAnchorableClosed;
                        documentViewBase.HostControl = layoutAnchorable;

                        // Create a DocumentView object passing the documentViewBase into its 
                        // constructor, where it gets set as the main content of the DocumentView.  
                        var documentView = new DocumentViewHost(documentViewBase);

                        // Then set the documentView as the content
                        // of the AvalonDock LayoutAnchorable control
                        // and add the LayoutAnchorable control to the 
                        // LayoutDocumentPane's childrens's collection,
                        // and set the LayoutAnchorable control as the 
                        // dockingManager's active content.
                        layoutAnchorable.Content = documentView;
                        layoutDocumentPane.Children.Add(layoutAnchorable);
                        dockingManager.ActiveContent = layoutAnchorable;
                    }
                }
            }
        }

        /// <summary>
        /// Removes the document from the <see cref="IRegion"/> after it has been closed.
        /// If you don't do this the document will not be removed but just hidden and you
        /// will not be able to open it again as prism will already think it exists.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">LayoutAnchorable closed event arguments.</param>
        private void LayoutAnchorableClosed(object sender, EventArgs e)
        {
            var layoutAnchorable = sender as LayoutAnchorable;
            if (layoutAnchorable == null)
            {
                return;
            }

            var view = ((DocumentViewHost)layoutAnchorable.Content).View;
            if (Region.Views.Contains(view))
            {
                view.CloseDocument();
                Region.RegionManager.Regions["DocumentRegion"].Remove(view);
            }
        }

        /// <summary>
        /// Keeps the active content of the <see cref="IRegion"/> in sync with the <see cref="HostControl"/>.
        /// </summary>
        /// <param name="sender">The <see cref="IRegion"/> views collection.</param>
        /// <param name="e">Event arguments.</param>
        private void ActiveViewsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (dockingManager.ActiveContent != null
                    && dockingManager.ActiveContent != e.NewItems[0]
                    && Region.ActiveViews.Contains(dockingManager.ActiveContent))
                {
                    Region.Deactivate(dockingManager.ActiveContent);
                }

                dockingManager.ActiveContent = e.NewItems[0];
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove 
                && e.OldItems.Contains(dockingManager.ActiveContent))
            {
                dockingManager.ActiveContent = null;
            }
        }

        /// <summary>
        /// Toggles between modules, showing documents available to the current 
        /// selected module, while hidding documents relating to other modules.
        /// </summary>
        /// <param name="sender">The <see cref="ModulesNavigationView"/>.</param>
        /// <param name="e">Module event arguments.</param>
        private void ModuleSelected(object sender, ModuleEventArgs e)
        {
            var layoutAnchorablesHide = dockingManager.Layout.Descendents()
                .OfType<LayoutAnchorable>()
                .Where(la => !((DocumentViewHost)la.Content).ModuleName.ToUpper().Equals(e.ModuleName.ToUpper()));

            var anchorablesHide = layoutAnchorablesHide.ToList();

            var layoutAnchorablesShow = dockingManager.Layout.Descendents()
                .OfType<LayoutAnchorable>()
                .Where(la => ((DocumentViewHost)la.Content).ModuleName.ToUpper().Equals(e.ModuleName.ToUpper()));

            var anchorablesShow = layoutAnchorablesShow.ToList();

            for (int i = 0; i < anchorablesHide.Count(); i++)
            {
                anchorablesHide[i].Hide();
                var documentViewHost = anchorablesHide[i].Content as DocumentViewHost;
                documentViewHost.View.OnActiveChanged(false);
            }

            for (int i = 0; i < anchorablesShow.Count(); i++)
            {
                anchorablesShow[i].Show();
            }
        }
    }
}
