﻿using businessLogic.ModelViews;
using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace businessLogic.Services.Interfaces
{
    public interface IDashboardService
    {
        DashboardViewModel GetDashboardData();
        EditViewModel GetEditViewModel(ApplicationUser user);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<IdentityResult> DeleteUserAsync(ApplicationUser user);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user, string newPassword);
        Task<IdentityResult> CreateUserAsync(CreateUserViewModel model);
        Task<IdentityResult> UpdateUserRoleAsync(ApplicationUser user, string roleId);

        Task<EditViewModel> GetEditViewModelAsync(ApplicationUser user);
    }
}
