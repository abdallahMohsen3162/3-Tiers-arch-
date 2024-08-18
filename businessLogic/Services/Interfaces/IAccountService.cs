using businessLogic.ModelViews;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace businessLogic.Services.Interfaces
{
    public interface IAccountService : IAuthService, IUserService
    {
        ProfileViewModel MapUserToProfileViewModel(ApplicationUser user);
    }
}
