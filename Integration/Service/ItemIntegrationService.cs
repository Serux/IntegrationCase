using Integration.Common;
using Integration.Backend;
using System.Collections.Concurrent;

namespace Integration.Service;

public sealed class ItemIntegrationService
{
    //This is a dependency that is normally fulfilled externally.
    private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();

    // If several itemContent (or their hashes) can be saved locally temporally without causing any issues, 
    // adding them to a dictionary shared by all threads can be used as a cache and is used to check in a fast O(1) 
    // way if the element is already being added to the IntegrationBackend.
    // Here, ConcurrentDictionary is used to not deal with thread locking.
    private static ConcurrentDictionary<string,byte> itemToSaveCache = new ConcurrentDictionary<string,byte>();

    // This is called externally and can be called multithreaded, in parallel.
    // More than one item with the same content should not be saved. However,
    // calling this with different contents at the same time is OK, and should
    // be allowed for performance reasons.
    public Result SaveItem(string itemContent)
    {
        // Check the backend to see if the content is already saved.
        // We add the item to the cache of elements that are being saved.
        // Use Tryadd to check if exists and add to the cache at the same time to avoid race conditions. 
        if (ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0 || !itemToSaveCache.TryAdd(itemContent,1))
        {
            return new Result(false, $"Duplicate item received with content {itemContent}.");
        }

        var item = ItemIntegrationBackend.SaveItem(itemContent);

        // Once it's saved to the IntegrationBackend, we free the memory of that element in the cache.
        itemToSaveCache.TryRemove(itemContent, out byte outvalue);

        return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
    }

    public List<Item> GetAllItems()
    {
        return ItemIntegrationBackend.GetAllItems();
    }
}