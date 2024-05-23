using authentication_service.DAL.Caching.CacheModels;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Runtime.Caching; 

namespace authentication_service.DAL.Caching
{
	public class CacheManager
	{
		private readonly TimeSpan _defaultExpieration;
		private const string CacheKey = "1";
		CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
		ObjectCache Cache = System.Runtime.Caching.MemoryCache.Default;
		public CacheManager()
		{
			cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(1.0);
		}

		public void AddCache()
		{
			var VALUE = new CacheUserModel
			{
				email = "no",
				AccessLevel = Enums.Userlevel.admin
			};
			Cache.Add(CacheKey, VALUE, cacheItemPolicy);
		}

		public CacheUserModel ReadCache()
		{

			var ReturnValue = Cache.Get(CacheKey);
			return (CacheUserModel)ReturnValue;
		}
	}
}
