using AutoMapper;
using businessLogic.ModelViews;
using businessLogic.Services.Interfaces;
using DataLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace businessLogic.Services.Services
{
    public class FileManager
    {
        private readonly string _rootPath;

        public FileManager(string rootPath)
        {
            _rootPath = rootPath;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty.", nameof(file));

            string uploadsFolder = Path.Combine(_rootPath, folderName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Path.Combine(folderName, uniqueFileName).Replace("\\", "/");
        }

        public bool DeleteFile(string relativeFilePath)
        {
            if (string.IsNullOrEmpty(relativeFilePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(relativeFilePath));

            string filePath = Path.Combine(_rootPath, relativeFilePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }
    }
}

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

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                FileManager _fileManager = new FileManager("wwwroot");
                string relativeFilePath = await _fileManager.SaveFileAsync(model.ProfileImage, "images/profiles");
                user.imageUrl = "/" + relativeFilePath;
            }

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
