using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;

namespace Jellyfin.Plugin.Webhook.Notifiers.ItemRemovedNotifier
{
    /// <summary>
    /// Item Removed manager interface.
    /// </summary>
    public interface IItemRemovedManager
    {
        /// <summary>
        /// Process the current queue.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task ProcessItemsAsync();

        /// <summary>
        /// Add item to process queue.
        /// </summary>
        /// <param name="item">The Removed item.</param>
        public void AddItem(BaseItem item);
    }
}