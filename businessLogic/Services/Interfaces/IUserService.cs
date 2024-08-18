using businessLogic.ModelViews;
using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace businessLogic.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        List<ApplicationUser> GetAllUsers();
        Task<IdentityResult> RegisterUser(RegisterViewModel model);
        Task<IdentityResult> DeleteUserAsync(ApplicationUser user);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    }
}
