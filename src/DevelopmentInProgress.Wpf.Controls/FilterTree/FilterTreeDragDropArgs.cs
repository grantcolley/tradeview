//-----------------------------------------------------------------------
// <copyright file="FilterTreeDragDropArgs.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2015. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DevelopmentInProgress.Wpf.Controls.FilterTree
{
    /// <summary>
    /// Arguments for a drag and drop operation in the <see cref="XamlFilterTree"/>.
    /// </summary>
    public class FilterTreeDragDropArgs
    {
        /// <summary>
        /// Initialises a new instance of the FilterTreeDragDropArgs class.
        /// </summary>
        /// <param name="dragItem">The item being dragged.</param>
        /// <param name="dropTarget">The target where the dragged item will be dropped.</param>
        public FilterTreeDragDropArgs(object dragItem, object dropTarget)
        {
            DragItem = dragItem;
            DropTarget = dropTarget;
        }

        /// <summary>
        /// Gets the object being dragged.
        /// </summary>
        public object DragItem { get; private set; }

        /// <summary>
        /// Gets the drop target for the object being dragged.
        /// </summary>
        public object DropTarget { get; private set; }
    }
}
