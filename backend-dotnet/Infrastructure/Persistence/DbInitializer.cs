using Domain.Contract;
using Domain.Entities.Identities;
using Domain.Entities.Orders;
using Domain.Entities.Product;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Context;
using Persistence.Identity.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Persistence
{
    //CLR
    public class DbInitializer(StoreDbContext _context,IdentityStoreDbContext _identityContext,UserManager<AppUser> _userManager,RoleManager<IdentityRole> _roleManager) : IDbinitializer
    {
        public async Task InitializeAsync()
        {
            //Create DB
            //Updata DB
            if(_context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Any())
            {
                await _context.Database.MigrateAsync();
            }
            //Product Brands
            if(!_context.ProductBrands.Any())
            {
                //Seed Data
                //1-Reed Data From Json File
                var bransdData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\DataSeeding\brands.json");
                //2-Deserialize Json Data
                var brand = JsonSerializer.Deserialize<List<ProductBrand>>(bransdData);
                //3-Add Data To DataBase
                if (brand is not null && brand.Count > 0)
                {
                    await _context.ProductBrands.AddRangeAsync(brand);
                }
            }
            //Product Types
            if (!_context.ProductTypes.Any())
            {
                //Seed Data
                //1-Reed Data From Json File
                var typesData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\DataSeeding\types.json");
                //2-Deserialize Json Data
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                //3-Add Data To DataBase
                if (types is not null && types.Count > 0)
                {
                    await _context.ProductTypes.AddRangeAsync(types);
                }
            }
            //Products
            if (!_context.Products.Any())
            {
                //Seed Data
                //1-Reed Data From Json File
                var productsdata = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\DataSeeding\products.json");
                //2-Deserialize Json Data
                var products = JsonSerializer.Deserialize<List<Product>>(productsdata);
                //3-Add Data To DataBase
                if (products is not null && products.Count > 0)
                {
                    await _context.Products.AddRangeAsync(products);
                }
            }
            //Order Delivery Methods
            if (!_context.DeliveryMethods.Any())
            {
                //Seed Data
                //1-Reed Data From Json File
                var deliveryMehod = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\DataSeeding\delivery.json");
                //2-Deserialize Json Data
                var delivery = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMehod);
                //3-Add Data To DataBase
                if (delivery is not null && delivery.Count > 0)
                {
                    await _context.DeliveryMethods.AddRangeAsync(delivery);
                }
            }

            await _context.SaveChangesAsync();
        }


        public async Task InitializeIdentityAsync()
        {
            //Create DB
            //Updata DB
            if (_identityContext.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Any())
            {
                await _identityContext.Database.MigrateAsync();
            }
            if(!_identityContext.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "SuperAdmin" });
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
            }
            if (!_identityContext.Users.Any())
            {
                var SuperAdmin = new AppUser()
                {
                    UserName = "SuperAdmin",
                    DisplayName = "SuperAdmin",
                    Email = "SuperAdmin@gmail.com",
                    PhoneNumber = "1234567890"
                };
                var Admin = new AppUser()
                {
                    UserName = "Admin",
                    DisplayName = "Admin",
                    Email = "Admin@gmail.com",
                    PhoneNumber = "1234567890"
                };
                await _userManager.CreateAsync(SuperAdmin, "P@ssw0rd"); ;
                await _userManager.CreateAsync(Admin, "P@ssw0rd");
                await _userManager.AddToRoleAsync(SuperAdmin, "SuperAdmin");
                await _userManager.AddToRoleAsync(Admin, "Admin");
            }
        }
    }
}
