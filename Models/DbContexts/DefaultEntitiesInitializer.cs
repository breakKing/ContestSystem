﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContestSystemDbStructure.Models;
using Microsoft.AspNetCore.Identity;

namespace ContestSystem.Models.DbContexts
{
    public static class DefaultEntitiesInitializer
    {
        private static readonly List<Role> Roles = new List<Role>()
        {
            new Role(RolesContainer.Admin)
            {
                Description = "Администратор"
            },
            new Role(RolesContainer.Moderator)
            {
                Description = "Модератор"
            },
            new Role(RolesContainer.User)
            {
                Description = "Пользователь"
            },
        };

        private static readonly UserModelForInitialization[] Users = new[]
        {
            new UserModelForInitialization()
            {
                UserName = "admin",
                Email = "admin@admin.ru",
                Password = "admin",
                Role = RolesContainer.Admin
            },
            new UserModelForInitialization()
            {
                UserName = "manager",
                Email = "manager@manager.ru",
                Password = "manager",
                Role = RolesContainer.Moderator,
            },
        };

        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            foreach (var role in Roles)
            {
                var existsRole = await roleManager.FindByNameAsync(role.Name);
                if (existsRole is null)
                {
                    await roleManager.CreateAsync(role);
                }
                else if (existsRole.Description != role.Description)
                {
                    existsRole.Description = role.Description;
                    await roleManager.UpdateAsync(existsRole);
                }
            }

            foreach (var userModel in Users)
            {
                if (await userManager.FindByNameAsync(userModel.UserName) is not null) continue;
                var user = new User
                {
                    Email = userModel.Email,
                    UserName = userModel.UserName,
                    Surname = userModel.UserName,
                    FirstName = userModel.UserName,
                    DateOfBirth = DateTime.Now,
                };
                var result = await userManager.CreateAsync(user, userModel.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, userModel.Role);
                }
            }
        }
    }
}