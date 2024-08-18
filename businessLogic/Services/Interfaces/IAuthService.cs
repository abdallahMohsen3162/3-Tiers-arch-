using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace businessLogic.Services.Interfaces
{
    public interface IAuthService
    {
        Task SignInUserAsync(ApplicationUser user, bool isPersistent = false);
        Task<SignInResult> SignInWithEmailAndPasswordAsync(string email, string password, bool rememberMe);
        Task SignOutUserAsync();
        bool IsUserAuthenticated(ClaimsPrincipal user);
    }
}
