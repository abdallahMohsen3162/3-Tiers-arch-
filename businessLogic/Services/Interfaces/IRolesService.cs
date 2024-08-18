using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace businessLogic.Services.Interfaces
{
    public interface IRolesService
    {

        SelectList getRoles();
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityRole> GetRoleByIdAsync(string roleId);
        Task<IdentityRole> GetRoleByNameAsync(string roleName);
        Task<IdentityResult> UpdateRoleAsync(string roleId, string newRoleName);
        Task<IdentityResult> DeleteRoleAsync(string roleId);

    }
}
