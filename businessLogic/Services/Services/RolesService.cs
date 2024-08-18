using businessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace businessLogic.Services.Services
{
    public class RolesService : IRolesService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public SelectList getRoles()
        {
            var ret = new SelectList(_roleManager.Roles.ToList(), "Id", "Name");
            return ret;
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            IdentityRole role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);
            return result;
        }

        public async Task<IdentityRole> GetRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        public async Task<IdentityRole> GetRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }

        public async Task<IdentityResult> UpdateRoleAsync(string roleId, string newRoleName)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
            }

            if (string.IsNullOrWhiteSpace(newRoleName))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role name cannot be empty." });
            }

            if (await _roleManager.RoleExistsAsync(newRoleName))
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{newRoleName}' already exists." });
            }

            role.Name = newRoleName;
            return await _roleManager.UpdateAsync(role);
        }
    }
}
