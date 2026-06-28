using Domain.Contract;
using Services.Abstractions.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Cache
{
    public class CacheService(ICacheRepository _cachRepository) : ICacheService
    {
        public async Task<string?> GetAsync(string key)
        {
            var result = await _cachRepository.GetAsync(key);
            return result;
        }

        public async Task SetAsync(string key, object value, TimeSpan duration)
        {
             await _cachRepository.SetAsync(key, value, duration);
        }
    }
}
