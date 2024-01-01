using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using Integration.Common;

namespace Integration.Backend;

public sealed class ItemOperationBackend
{
    private ConcurrentDictionary<string,Item> SavedItems { get; set; } = new();
    private int _identitySequence;

    public Item SaveItem(string itemContent)
    {
        // This simulates how long it takes to save
        // the item content. Forty seconds, give or take.
        Thread.Sleep(2_000);
        
        var item = new Item();
        item.Content = itemContent;
        item.Id = GetNextIdentity();

        // Tries to add the new item. If cannot add the item (because it is already saved), 
        // then it returns to the caller the item that was already saved.
        if(!SavedItems.TryAdd(itemContent,item))
        {
            item = SavedItems.Where(x=> x.Key == itemContent).FirstOrDefault().Value;
        }

        return item;
    }

    public List<Item> FindItemsWithContent(string itemContent)
    {
        return SavedItems.Where(x => x.Key == itemContent).Select(x=> x.Value).ToList();
    }

    private int GetNextIdentity()
    {
        return Interlocked.Increment(ref _identitySequence);
    }

    public List<Item> GetAllItems()
    {
        return SavedItems.Values.ToList();
    }
}