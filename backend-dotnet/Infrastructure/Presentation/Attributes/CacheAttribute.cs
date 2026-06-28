using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;
using System.Text;

namespace Presentation.Attributes
{
    public class CacheAttribute(int timeInSec) : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var cacheService =  context.HttpContext.RequestServices.GetRequiredService<IServiceManager>().CacheService;
           var key = GetCacheKey(context.HttpContext.Request);
           var cachedResponse = await cacheService.GetAsync(key);
           if(!string.IsNullOrEmpty(cachedResponse))
            {
                var response = new ContentResult()
                { 
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = response;
                return;
           }
            var actionContext = await next.Invoke();
            if(actionContext.Result is OkObjectResult okObjectResult)
            {
                await cacheService.SetAsync(key,okObjectResult.Value,TimeSpan.FromSeconds(timeInSec));
            }
        }
        private string GetCacheKey(HttpRequest request)
        {
            var key = new StringBuilder();
            key.Append(request.Path);
            foreach (var item in request.Query)
            {
                key.Append($"|{item.Key}-{item.Value}|");
            }
            return key.ToString();
        }
    }
}
