namespace GardenLogWeb.Services;

public interface ICacheService
{
    TItem Set<TItem>(object key, TItem value);
    TItem Set<TItem>(object key, TItem value, DateTime expireAfter);
    bool TryGetValue<TItem>(object key, out TItem value);
}

public class CacheService : ICacheService
{
    private Dictionary<object, CacheItem> _cache = new();

    public bool TryGetValue<TItem>(object key, out TItem value)
    {
        if (_cache.ContainsKey(key))
        {
            var cacheItem = (CacheItem)_cache[key];
            if (cacheItem.ExporeAfter.HasValue && cacheItem.ExporeAfter.Value < DateTime.Now)
            {
                _cache.Remove(key);
            }
            else
            {
                value = (TItem)cacheItem.Item;
                return true;
            }
        }

        value = default(TItem);
        return false;

    }
    public TItem Set<TItem>(object key, TItem value)
    {
        if (_cache.ContainsKey(key))
        {
            _cache[key] = new CacheItem(value, null);
        }
        else
        {
            _cache.Add(key, new CacheItem(value, null));
        }
        return value;
    }

    public TItem Set<TItem>(object key, TItem value, DateTime expireAfter)
    {
        if (_cache.ContainsKey(key))
        {
            _cache[key] = new CacheItem(value, expireAfter);
        }
        else
        {
            _cache.Add(key, new CacheItem(value, expireAfter));
        }
        return value;
    }

    private record CacheItem(Object Item, DateTime? ExporeAfter);

}
