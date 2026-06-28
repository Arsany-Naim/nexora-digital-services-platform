using Domain.Contract;
using Domain.Entities.Identities;
using Domain.Exceptions;
using E_Commerce.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using Persistence.Identity.Context;
using Services;
using Shared;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.ExtensionsMethod
{
    public static class Extensions
    {
        #region ConfigureAllServices
        public static IServiceCollection AddAllServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddWebServices();
            services.AddInfrastructureServices(configuration);
            services.AddApplicationServices(configuration);
            services.ConfigureApiBehaviorOptions();
            services.AddIdentityServices();
            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            services.AddAuthenticationService(configuration);
            services.AddCors(Options =>
            {
                Options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            return services;
        }

        private static IServiceCollection ConfigureApiBehaviorOptions(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(config =>
            {
                config.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var error = actionContext.ModelState.Where(M => M.Value.Errors.Any())
                                                                                .Select(M => new ValidationErrorr()
                                                                                {
                                                                                    Field = M.Key,
                                                                                    Error = M.Value.Errors.Select(E => E.ErrorMessage)
                                                                                }).ToList();
                    var response = new ValidationErrorResponse()
                    {
                        Errors = error
                    };
                    return new BadRequestObjectResult(response);
                };
            });
            return services;
        }
        private static IServiceCollection AddAuthenticationService(this IServiceCollection services , IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey))
                };
            });
            return services;
        }

        private static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }
        private static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentityCore<AppUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
            }).AddRoles<IdentityRole>().AddEntityFrameworkStores<IdentityStoreDbContext>();
            return services;
        }
        #endregion

        #region ConfigureAllMidlleWare
        public static async Task<WebApplication> ConfigureAllMiddlewares(this WebApplication app)
        {
            #region InitializeDB
            await app.SeedData();
            #endregion

            app.UseGlobalErrorHandling();
            app.UseStaticFiles();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            return app;
        }

        private static WebApplication UseGlobalErrorHandling(this WebApplication app)
        {
            app.UseMiddleware<GlobalErrorHandlingMiddleware>();
            return app;
        }

        private static async Task<WebApplication> SeedData(this WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbinitializer>(); // Ask CLR to Create Object from IDbinitializer
            await dbInitializer.InitializeAsync();
            await dbInitializer.InitializeIdentityAsync();
            return app;
        } 
        #endregion
    }
}
