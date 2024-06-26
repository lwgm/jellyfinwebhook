using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Jellyfin.Plugin.Webhook.Destinations;
using Jellyfin.Plugin.Webhook.Helpers;
using Jellyfin.Plugin.Webhook.Models;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Webhook.Notifiers.ItemRemovedNotifier
{
    /// <inheritdoc />
    public class ItemRemovedManager : IItemRemovedManager
    {
        private readonly ILogger<ItemRemovedManager> _logger;
        private readonly ILibraryManager _libraryManager;
        private readonly IServerApplicationHost _applicationHost;
        private readonly IWebhookSender _webhookSender;
        private readonly ConcurrentDictionary<Guid, BaseItem> _itemProcessQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRemovedManager"/> class.
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger{ItemRemovedManager}"/> interface.</param>
        /// <param name="libraryManager">Instance of the <see cref="ILibraryManager"/> interface.</param>
        /// <param name="applicationHost">Instance of the <see cref="IServerApplicationHost"/> interface.</param>
        /// <param name="webhookSender">Instance of the <see cref="IWebhookSender"/> interface.</param>
        public ItemRemovedManager(
            ILogger<ItemRemovedManager> logger,
            ILibraryManager libraryManager,
            IServerApplicationHost applicationHost,
            IWebhookSender webhookSender)
        {
            _logger = logger;
            _libraryManager = libraryManager;
            _applicationHost = applicationHost;
            _webhookSender = webhookSender;
            _itemProcessQueue = new ConcurrentDictionary<Guid, BaseItem>();
        }

        /// <inheritdoc />
        public async Task ProcessItemsAsync()
        {
            _logger.LogError("ProcessItemsAsync");
            // Attempt to process all items in queue.
            // var currentItems = _itemProcessQueue.ToArray();
            foreach (var (key, item) in _itemProcessQueue)
            {
                var dataObject = DataObjectHelpers
                    .GetBaseDataObject(_applicationHost, NotificationType.ItemRemoved)
                    .AddBaseItemData(item);
                dataObject[nameof(item.Path)] = item.Path;
                if (string.IsNullOrEmpty(item.Path))
                {
                    _logger.LogError("item {0} has no path", item.Name);
                    return; // no notification send
                }

                var itemexist = _libraryManager.GetItemById(key);
                if (itemexist is not null)
                {
                    _logger.LogError("item {Itemname} still exist", item.Name);
                    return;
                }

                var itemType = item.GetType();
                await _webhookSender.SendNotification(NotificationType.ItemRemoved, dataObject, itemType)
                    .ConfigureAwait(false);
                // Remove item from queue.
                _itemProcessQueue.TryRemove(key, out _);
            }
        }

        /// <inheritdoc />
        public void AddItem(BaseItem item)
        {
            _itemProcessQueue.TryAdd(item.Id, item);
            _logger.LogError("Queued {0} for notification, its path is {1}", item.Name, item.Path);
        }
    }
}
