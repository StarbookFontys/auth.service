using authentication_service.DAL.Caching.CacheModels;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching;

namespace authentication_service.DAL.Caching
{
	public class CacheManager<TValue>
	{
		private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

		public TValue GetOrCreate(object key, Func<TValue> createItem)
		{
			TValue cacheEntry;
			if (!_cache.TryGetValue(key, out cacheEntry))// Look for cache key.
			{
				// Key not in cache, so get data.
				cacheEntry = createItem();

				var cacheEntryOptions = new MemoryCacheEntryOptions()
					.SetSize(1)//Size amount
					//Priority on removing when reaching size limit (memory pressure)
					.SetPriority(CacheItemPriority.High)
					// Keep in cache for this time, reset time if accessed.
					.SetSlidingExpiration(TimeSpan.FromSeconds(30))
					// Remove from cache after this time, regardless of sliding expiration
					.SetAbsoluteExpiration(TimeSpan.FromSeconds(120));

				// Save data in cache.
				_cache.Set(key, cacheEntry, cacheEntryOptions);
			}
			return cacheEntry;
		}
	}
}
