using Domain.Exceptions.BadRequest;
using Domain.Exceptions.NotFound;
using Domain.Exceptions.UnAuthorized;
using Shared.ErrorModels;

namespace E_Commerce.Middlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                if(context.Response.StatusCode == 404)
                {
                    context.Response.ContentType = "application/json";
                    var response = new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        ErrorMessage = $"The End Point : {context.Request.Path} Is Not Found"
                    };
                    await context.Response.WriteAsJsonAsync(response);
                }
            }
            catch(Exception ex)
            {
                //Logic
                //1-Set Status Code of Response
                context.Response.StatusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    BadRequestexception => StatusCodes.Status400BadRequest,
                    UnAuthorizedExcpetion => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError
                };
                //2-Set Content Type of Response
                context.Response.ContentType = "application/json";
                //3-Set Body of Response
                var response = new ErrorDetails()
                {
                    StatusCode = context.Response.StatusCode,
                    ErrorMessage = ex.Message
                };
                //4-Return Response
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
