using Domain.Contract;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class CacheRepository(IConnectionMultiplexer connection) : ICacheRepository
    {
        private readonly IDatabase _database = connection.GetDatabase();
        public async Task<string?> GetAsync(string key)
        {
           var redis = await _database.StringGetAsync(key);
           return redis;
        }

        public async Task SetAsync(string key, object value, TimeSpan duration)
        {
                await _database.StringSetAsync(key,JsonSerializer.Serialize(value),duration);
        }
    }
}
