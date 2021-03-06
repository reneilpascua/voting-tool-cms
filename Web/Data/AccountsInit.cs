﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Web.Models.Constants;

namespace Web.Data
{
    public static class AccountsInit
    {
        public static UserManager<IdentityUser> userManager;
        public static RoleManager<IdentityRole> roleManager;

        public static async void InitializeAsync(IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                userManager = serviceScope.ServiceProvider.GetService<UserManager<IdentityUser>>();
                roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                await InsertUserAsync().ConfigureAwait(false);
            }
        }

        public static async Task InsertUserAsync()
        {
            await CreateRole(
                Account.ROLE_ADMIN,
                Account.ROLE_ADMIN);
            await AddNewUserToRole(
                Account.ADMIN_EMAIL,
                Account.ADMIN_USERNAME,
                Account.DEFAULT_PASSWORD,
                Account.ROLE_ADMIN);

            await CreateRole(
                Account.ROLE_EDITOR,
                Account.ROLE_EDITOR);
            await AddNewUserToRole(
                Account.EDITOR_EMAIL,
                Account.EDITOR_USERNAME,
                Account.DEFAULT_PASSWORD,
                Account.ROLE_EDITOR);
        }

        private static async Task CreateRole(string identityRoleName, string identityRoleNormalizedName)
        {
            var role = new IdentityRole { Name = identityRoleName, NormalizedName = identityRoleNormalizedName };

            if (await roleManager.FindByNameAsync(role.Name) == null)
            {
                await roleManager.CreateAsync(role);
            }
        }

        private static async Task AddNewUserToRole(string email, string userName, string password, string role)
        {
            var user = new IdentityUser { Email = email, UserName = userName, SecurityStamp = Guid.NewGuid().ToString() };

            if ((await userManager.CreateAsync(user, password)).Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
