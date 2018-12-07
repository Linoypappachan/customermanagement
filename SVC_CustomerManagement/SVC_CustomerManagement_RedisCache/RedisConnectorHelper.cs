using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVC_CustomerManagement_RedisCache
{
    public class RedisConnectorHelper
    {
        private static double CacheExpiry { get { return 0.3; } }
        static RedisConnectorHelper()
        {
           
            RedisConnectorHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect("localhost:6379");
            });
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public static void GetCacheData(string cacheKey, out StackExchange.Redis.IDatabase cache, out StackExchange.Redis.RedisValue cachedCustdetails)
        {
            cache = Connection.GetDatabase();
            cachedCustdetails = cache.StringGet(cacheKey);
            cache.KeyExpire(cacheKey, TimeSpan.FromMinutes(CacheExpiry));
        }
    }  
}
