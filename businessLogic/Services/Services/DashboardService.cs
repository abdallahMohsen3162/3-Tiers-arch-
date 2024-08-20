using AutoMapper;
using businessLogic.ModelViews;
using businessLogic.Services.Interfaces;
using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using businessLogic.Services.Services;

namespace businessLogic.Services.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly FileManager _fileManager;

        public ClaimsPrincipal User { get; private set; }

        public DashboardService(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _fileManager = new FileManager("wwwroot");
        }

        public DashboardViewModel GetDashboardData()
        {
            var allUsers = _userManager.Users.ToArray();
            foreach (var user in allUsers)
            {
                Console.WriteLine(user.Email);
            }
            var profileViewModels = new List<ProfileViewModel>();

            foreach (var user in allUsers)
            {
                var profileViewModel = _mapper.Map<ProfileViewModel>(user);
                var roles = _userManager.GetRolesAsync(user).Result;

                profileViewModel.Role = roles.Any() ? string.Join(", ", roles) : "No Role Assigned";

                profileViewModels.Add(profileViewModel);
            }

            return new DashboardViewModel
            {
                Users = profileViewModels.ToArray(),
            };
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            if (!string.IsNullOrEmpty(user.imageUrl))
            {
                _fileManager.DeleteFile(user.imageUrl.TrimStart('/'));
            }

            return await _userManager.DeleteAsync(user);
        }

        public EditViewModel GetEditViewModel(ApplicationUser user)
        {
            return new EditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Address = user.Address,
                Age = user.Age ?? 0,
            };
        }

        public async Task<List<Claim>> GetUserClaimsAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                Console.WriteLine(id);
                Console.WriteLine("User not found");
                return new List<Claim>();
            }
            return (await _userManager.GetClaimsAsync(user)).ToList();
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user, string newPassword)
        {
            if (!string.IsNullOrEmpty(newPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!result.Succeeded)
                {
                    return result;
                }
            }

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> CreateUserAsync(CreateUserViewModel model)
        {
            var user = _mapper.Map<ApplicationUser>(model);

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                string relativeFilePath = await _fileManager.SaveFileAsync(model.ProfileImage, "images/profiles");
                user.imageUrl = "/" + relativeFilePath;
            }

            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (createResult.Succeeded && !string.IsNullOrEmpty(model.RoleId))
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                if (role != null)
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(user, role.Name);
                    if (!addToRoleResult.Succeeded)
                    {
                        return addToRoleResult;
                    }
                }
            }

            return createResult;
        }

        public async Task<IdentityResult> UpdateUserRoleAsync(ApplicationUser user, string roleId)
        {
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);

            var newRole = await _roleManager.FindByIdAsync(roleId);
            if (newRole != null)
            {
                return await _userManager.AddToRoleAsync(user, newRole.Name);
            }

            return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
        }

        public async Task<EditViewModel> GetEditViewModelAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleId = string.Empty;

            if (roles.Any())
            {
                var role = await _roleManager.FindByNameAsync(roles.First());
                roleId = role?.Id;
            }

            return new EditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Address = user.Address,
                Age = user.Age ?? 0,
                RoleId = roleId
            };
        }

        public async Task<string[]> GetUserRolesAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToArray();
        }

        public async Task<bool> UpdateUserClaimsAsync(string userId, List<UserClaim> claims)
        {
            foreach (var claim in claims)
            {
                Console.WriteLine(claim.ClaimType + " " + claim.IsSelected);
            }
            var user = await GetUserByIdAsync(userId);
            if (user == null) return false;

            var currentClaims = await GetUserClaimsAsync(userId);

            var removeResult = await _userManager.RemoveClaimsAsync(user, currentClaims);
            if (!removeResult.Succeeded) return false;

            var selectedClaims = claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.ClaimType));
            var addResult = await _userManager.AddClaimsAsync(user, selectedClaims);
            return addResult.Succeeded;
        }
    }
}
