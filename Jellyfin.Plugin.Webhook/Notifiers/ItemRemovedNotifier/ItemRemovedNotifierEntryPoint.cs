using System;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Webhook.Helpers;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Webhook.Notifiers.ItemRemovedNotifier
{
    /// <summary>
    /// Notifier when a library item is Removed.
    /// </summary>
    public class ItemRemovedNotifierEntryPoint : IHostedService
    {
        private readonly IItemRemovedManager _itemRemovedManager;
        private readonly ILibraryManager _libraryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRemovedNotifierEntryPoint"/> class.
        /// </summary>
        /// <param name="itemRemovedManager">Instance of the <see cref="IItemRemovedManager"/> interface.</param>
        /// <param name="libraryManager">Instance of the <see cref="ILibraryManager"/> interface.</param>
        public ItemRemovedNotifierEntryPoint(
            IItemRemovedManager itemRemovedManager,
            ILibraryManager libraryManager)
        {
            _itemRemovedManager = itemRemovedManager;
            _libraryManager = libraryManager;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _libraryManager.ItemRemoved += ItemRemovedHandler;
            return Task.CompletedTask;
        }

       /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _libraryManager.ItemRemoved -= ItemRemovedHandler;
            return Task.CompletedTask;
        }

        private void ItemRemovedHandler(object? sender, ItemChangeEventArgs itemChangeEventArgs)
        {
            // Never notify on virtual items.
            if (itemChangeEventArgs.Item.IsVirtualItem)
            {
                return;
            }

            _itemRemovedManager.AddItem(itemChangeEventArgs.Item);
        }
    }
}