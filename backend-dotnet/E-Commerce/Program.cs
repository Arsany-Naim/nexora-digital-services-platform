
using Domain.Contract;
using Domain.Exceptions;
using E_Commerce.ExtensionsMethod;
using E_Commerce.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Data.Context;
using Services;
using Services.Abstractions;
using Services.Mapping.Products;
using System.Threading.Tasks;

namespace E_Commerce
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddAllServices(builder.Configuration);
            var app = builder.Build();
            await app.ConfigureAllMiddlewares();
            app.Run();
        }
    }
}
