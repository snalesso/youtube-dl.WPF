using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using youtube_dl.WPF.Presentation.ViewModels;

namespace youtube_dl.WPF.Presentation.Behaviors
{
    class TreeViewDownloadQueueEntryViewModelBehavior : Behavior<TreeView>
    {
        /// <summary>
        ///     Identifies the <see cref="SelectedItem" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(object),
                typeof(DownloadQueueEntryViewModel),
                new UIPropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        ///     Gets or sets the selected item of the <see cref="TreeView" /> that this behavior is attached
        ///     to.
        /// </summary>
        public object SelectedItem
        {
            get { return this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        ///     Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        ///     Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.SelectedItemChanged += this.OnTreeViewSelectedItemChanged;
        }

        /// <summary>
        ///     Called when the behavior is being detached from its AssociatedObject, but before it has
        ///     actually occurred.
        /// </summary>
        /// <remarks>
        ///     Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.SelectedItemChanged -= this.OnTreeViewSelectedItemChanged;
            }
        }

        private static Action<int> GetBringIndexIntoView(Panel itemsHostPanel)
        {
            if (!(itemsHostPanel is VirtualizingStackPanel virtualizingPanel))
            {
                return null;
            }

            var method = virtualizingPanel.GetType().GetMethod(
                "BringIndexIntoView",
                BindingFlags.Instance | BindingFlags.NonPublic,
                Type.DefaultBinder,
                new[] { typeof(int) },
                null);

            if (method == null)
            {
                return null;
            }

            return i => method.Invoke(virtualizingPanel, new object[] { i });
        }

        /// <summary>
        /// Recursively search for an item in this subtree.
        /// </summary>
        /// <param name="treeViewItem">
        /// The parent ItemsControl. This can be a TreeView or a TreeViewItem.
        /// </param>
        /// <param name="route">
        /// The item to search for.
        /// </param>
        /// <returns>
        /// The TreeViewItem that contains the specified item.
        /// </returns>
        private static TreeViewItem GetTreeViewItem(ItemsControl container, IReadOnlyList<object> route, int routeIndex)
        {
            if (container == null)
                return null;

            switch (container)
            {
                case TreeView treeView:
                    for (int i = 0; i < treeView.Items.Count; i++)
                    {
                        var treeViewItem = treeView.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;

                        if (treeViewItem == null || treeViewItem.DataContext != route[routeIndex])
                        {
                            continue;
                        }

                        if (routeIndex == 0)
                            return treeViewItem;

                        return GetTreeViewItem(treeViewItem, route, --routeIndex);
                    }
                    break;

                case TreeViewItem treeViewItem:

                    if (treeViewItem.IsExpanded == false)
                    {
                        treeViewItem.SetValue(TreeViewItem.IsExpandedProperty, true);
                    }

                    // Try to generate the ItemsPresenter and the ItemsPanel.
                    // by calling ApplyTemplate.  Note that in the 
                    // virtualizing case even if the item is marked 
                    // expanded we still need to do this step in order to 
                    // regenerate the visuals because they may have been virtualized away.
                    treeViewItem.ApplyTemplate();
                    var itemsPresenter = (ItemsPresenter)treeViewItem.Template.FindName("ItemsHost", treeViewItem);
                    if (itemsPresenter != null)
                    {
                        itemsPresenter.ApplyTemplate();
                    }
                    else
                    {
                        // The Tree template has not named the ItemsPresenter, 
                        // so walk the descendents and find the child.
                        itemsPresenter = treeViewItem.GetVisualDescendant<ItemsPresenter>();
                        if (itemsPresenter == null)
                        {
                            treeViewItem.UpdateLayout();
                            itemsPresenter = treeViewItem.GetVisualDescendant<ItemsPresenter>();
                        }
                    }

                    var itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

                    // Ensure that the generator for this panel has been created.
                    var children = itemsHostPanel.Children;

                    var bringIndexIntoView = GetBringIndexIntoView(itemsHostPanel);
                    for (int i = 0, count = treeViewItem.Items.Count; i < count; i++)
                    {
                        TreeViewItem childTreeViewItem;
                        if (bringIndexIntoView != null)
                        {
                            // Bring the item into view so 
                            // that the container will be generated.
                            bringIndexIntoView(i);
                            childTreeViewItem = (TreeViewItem)treeViewItem.ItemContainerGenerator.ContainerFromIndex(i);
                        }
                        else
                        {
                            childTreeViewItem = (TreeViewItem)treeViewItem.ItemContainerGenerator.ContainerFromIndex(i);

                            // Bring the item into view to maintain the 
                            // same behavior as with a virtualizing panel.
                            childTreeViewItem.BringIntoView();
                        }

                        if (childTreeViewItem == null || childTreeViewItem.DataContext != route[routeIndex])
                        {
                            continue;
                        }

                        if (routeIndex == 0)
                            return childTreeViewItem;

                        return GetTreeViewItem(childTreeViewItem, route, --routeIndex);
                    }
                    break;

                default:
                    throw new NotSupportedException(); // TODO: log & make non-harmful
            }

            return null;
        }

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is TreeViewItem item)
            {
                item.SetValue(TreeViewItem.IsSelectedProperty, true);
                return;
            }

            var behavior = (TreeViewDownloadQueueEntryViewModelBehavior)sender;
            var treeView = behavior.AssociatedObject;
            if (treeView == null)
            {
                // at designtime the AssociatedObject sometimes seems to be null
                return;
            }

            if (!(e.NewValue is DownloadQueueEntryViewModel dataContext))
            {
                return;
            }

            var dataContextReversedRoute = new List<DownloadQueueEntryViewModel>();
            var parent = dataContext;
            while (parent != null)
            {
                dataContextReversedRoute.Add(parent);
                parent = parent.ParentTracksSubsetViewModel;
            }

            item = GetTreeViewItem(treeView, dataContextReversedRoute, dataContextReversedRoute.Count - 1);
            if (item != null)
            {
                item.IsSelected = true;
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedItem = e.NewValue;
        }
    }
}