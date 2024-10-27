using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaDemo.DataAccess.Data;
using PizzaDemo.Models;
using PizzaDemo.Utility;

namespace PizzaDemo.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
            ILogger<DbInitializer> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
            _logger = logger;
        }
        public async Task InitializeAsync()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _logger.LogInformation("開始應用數據庫遷移...");
                    await _db.Database.MigrateAsync();
                    _logger.LogInformation("數據庫遷移完成。");
                }

                await SeedRolesAsync();
                await SeedAdminUserAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "初始化數據庫時發生錯誤");
                throw; // 重新拋出異常，以便調用者知道初始化失敗
            }
        }

        private async Task SeedRolesAsync()
        {
            string[] roleNames = { SD.Role_Admin, SD.Role_Employee, SD.Role_Manager, SD.Role_Customer };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await CreateRoleAsync(roleName);
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminEmail = "admin@gmail.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Administrator",
                    PhoneNumber = "0911111111",
                    Address = "test address 123",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123*");
                if (result.Succeeded)
                {
                    _logger.LogInformation("管理員用戶創建成功");
                    await _userManager.AddToRoleAsync(adminUser, SD.Role_Admin);
                }
                else
                {
                    _logger.LogError("創建管理員用戶失敗: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        private async Task CreateRoleAsync(string roleName)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                _logger.LogInformation("角色 {RoleName} 創建成功", roleName);
            }
            else
            {
                _logger.LogError("創建角色 {RoleName} 失敗: {Errors}", roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
