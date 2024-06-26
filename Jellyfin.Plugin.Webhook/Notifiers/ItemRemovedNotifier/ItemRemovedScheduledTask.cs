using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Globalization;
using MediaBrowser.Model.Tasks;

namespace Jellyfin.Plugin.Webhook.Notifiers.ItemRemovedNotifier
{
    /// <summary>
    /// Scheduled task that processes item Removed events.
    /// </summary>
    public class ItemRemovedScheduledTask : IScheduledTask, IConfigurableScheduledTask
    {
        private const int RecheckIntervalSec = 30;
        private readonly IItemRemovedManager _itemRemovedManager;
        private readonly ILocalizationManager _localizationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRemovedScheduledTask"/> class.
        /// </summary>
        /// <param name="itemRemovedManager">Instance of the <see cref="IItemRemovedManager"/> interface.</param>
        /// <param name="localizationManager">Instance of the <see cref="ILocalizationManager"/> interface.</param>
        public ItemRemovedScheduledTask(
            IItemRemovedManager itemRemovedManager,
            ILocalizationManager localizationManager)
        {
            _itemRemovedManager = itemRemovedManager;
            _localizationManager = localizationManager;
        }

        /// <inheritdoc />
        public string Name => "Webhook Item Removed Notifier";

        /// <inheritdoc />
        public string Key => "WebhookItemRemoved";

        /// <inheritdoc />
        public string Description => "Processes item Removed queue";

        /// <inheritdoc />
        public string Category => _localizationManager.GetLocalizedString("TasksLibraryCategory");

        /// <inheritdoc />
        public bool IsHidden => false;

        /// <inheritdoc />
        public bool IsEnabled => true;

        /// <inheritdoc />
        public bool IsLogged => false;

        /// <inheritdoc />
        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            return _itemRemovedManager.ProcessItemsAsync();
        }

        /// <inheritdoc />
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
            {
                new TaskTriggerInfo
                {
                    Type = TaskTriggerInfo.TriggerInterval,
                    IntervalTicks = TimeSpan.FromSeconds(RecheckIntervalSec).Ticks
                }
            };
        }
    }
}
