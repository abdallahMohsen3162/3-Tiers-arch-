using AutoMapper;
using businessLogic.ModelViews;
using businessLogic.Services.Interfaces;
using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace businessLogic.Services.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public DashboardService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public DashboardViewModel GetDashboardData()
        {
            var allUsers = _userManager.Users.ToArray();
            var profileViewModels = _mapper.Map<ProfileViewModel[]>(allUsers);

            DashboardViewModel model = new DashboardViewModel
            {
                Users = profileViewModels,
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
                Age = user.Age != null ? (int)user.Age : 0
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
            return await _userManager.CreateAsync(user, model.Password);
        }
    }
}
