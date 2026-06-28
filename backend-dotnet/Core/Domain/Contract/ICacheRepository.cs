using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contract
{
    public interface ICacheRepository
    {
        Task<string?> GetAsync(string key);
        Task SetAsync(string key,object value,TimeSpan duration);
    }
}
