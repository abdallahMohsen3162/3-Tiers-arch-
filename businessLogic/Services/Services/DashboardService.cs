using AutoMapper;
using businessLogic.ModelViews;
using businessLogic.Services.Interfaces;
using DataLayer.Entities;
using DataLayer.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace businessLogic.Services.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public ClaimsPrincipal User { get; private set; }

        public DashboardService(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;

        }

        public DashboardViewModel GetDashboardData()
        {



            var allUsers = _userManager.Users.ToArray();
            var profileViewModels = new List<ProfileViewModel>();

            foreach (var user in allUsers)
            {
                var profileViewModel = _mapper.Map<ProfileViewModel>(user);
                var roles = _userManager.GetRolesAsync(user).Result; // Fetch roles for each user

                if (roles.Any())
                {
                    profileViewModel.Role = string.Join(", ", roles); // Join multiple roles with commas, if necessary
                }
                else
                {
                    profileViewModel.Role = "No Role Assigned"; // Handle users with no roles
                }

                profileViewModels.Add(profileViewModel);
            }

            DashboardViewModel model = new DashboardViewModel
            {
                Users = profileViewModels.ToArray(),
            };

            return model;
        }


        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public EditViewModel GetEditViewModel(ApplicationUser user)
        {

                
            return new EditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Address = user.Address,
                Age = user.Age != null ? (int)user.Age : 0,
            };
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
                Age = user.Age != null ? (int)user.Age : 0,
                RoleId = roleId
            };
        }
    }
}
