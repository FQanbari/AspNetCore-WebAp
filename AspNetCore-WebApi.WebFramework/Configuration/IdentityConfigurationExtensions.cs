using AspNetCore_WebApi.Common;
using AspNetCore_WebApi.Data;
using AspNetCore_WebApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore_WebApi.WebFramework.Configuration
{
    public static class IdentityConfigurationExtensions
    {
        public static void AddCustomIdentity(this IServiceCollection services, IdentitySettings settings)
        {
            services.AddIdentity<User, Role>(identityOptions =>
            {
                //Password Settings
                identityOptions.Password.RequireDigit = settings.PasswordRequireDigit;
                identityOptions.Password.RequiredLength = settings.PasswordRequiredLength;
                identityOptions.Password.RequireNonAlphanumeric = settings.PasswordRequireNonAlphanumic; //#@!
                identityOptions.Password.RequireUppercase = settings.PasswordRequireUppercase;
                identityOptions.Password.RequireLowercase = settings.PasswordRequireLowercase;

                //UserName Settings
                identityOptions.User.RequireUniqueEmail = settings.RequireUniqueEmail;

                //Singin Settings
                //identityOptions.SignIn.RequireConfirmedEmail = false;
                //identityOptions.SignIn.RequireConfirmedPhoneNumber = false;

                //Lockout Settings
                //identityOptions.Lockout.MaxFailedAccessAttempts = 5;
                //identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                //identityOptions.Lockout.AllowedForNewUsers = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }
    }
}
