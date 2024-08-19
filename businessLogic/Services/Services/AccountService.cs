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
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public async Task<ApplicationUser> GetUserAsync(System.Security.Claims.ClaimsPrincipal user)
        {
            return await _userManager.GetUserAsync(user);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {

            return await _userManager.FindByEmailAsync(email);
        }

        public ProfileViewModel MapUserToProfileViewModel(ApplicationUser user)
        {
            return _mapper.Map<ProfileViewModel>(user);
        }

        public List<ApplicationUser> GetAllUsers()
        {
            return _userManager.Users.ToList();
        }

        public async Task<IdentityResult> RegisterUser(RegisterViewModel model)
        {
            var user = _mapper.Map<ApplicationUser>(model);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return result;
        }


        public async Task SignInUserAsync(ApplicationUser user, bool isPersistent = false)
        {
            await _signInManager.SignInAsync(user, isPersistent);
        }

        public bool IsUserAuthenticated(System.Security.Claims.ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated;
        }

        public async Task<SignInResult> SignInWithEmailAndPasswordAsync(string email, string password, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
        }

        public async Task SignOutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}
