using businessLogic.ModelViews;
using businessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataLayer.Entities;
namespace Interface.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IRolesService roleService;

        public DashboardController(IDashboardService dashboardService, IRolesService roleService)
        {
            _dashboardService = dashboardService;
            this.roleService = roleService;
        }


        //[Authorize(Policy = AuthenticationConstants.Identity.Manage)]
        public IActionResult Index()
        {
            var model = _dashboardService.GetDashboardData();
            return View(model);
        }

        //[Authorize(Policy = AuthenticationConstants.Identity.Delete)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _dashboardService.GetUserByIdAsync(id);

            if (user == null )
            {
                return NotFound();
            }

            return View(user);
        }

        //[Authorize(Policy = AuthenticationConstants.Identity.Delete)]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _dashboardService.GetUserByIdAsync(id);
            var role = await _dashboardService.GetUserRolesAsync(user);
            bool isAdmin = role.Contains("Admin");
            if (user != null || isAdmin)
            {
                await _dashboardService.DeleteUserAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        //[Authorize(Policy = AuthenticationConstants.Identity.Edit)]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null )
            {
                return NotFound();
            }

            var user = await _dashboardService.GetUserByIdAsync(id);


            if (user == null)
            {
                return NotFound();
            }

            var model = await _dashboardService.GetEditViewModelAsync(user);
            ViewBag.RoleList = roleService.getRoles();
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("EditConfirmed")]
        [Authorize]
        public async Task<IActionResult> Edit(string id, EditViewModel model)
        {
            if (string.IsNullOrEmpty(model.NewPassword) && string.IsNullOrEmpty(model.ConfirmPassword))
            {
                ModelState.Remove("NewPassword");
                ModelState.Remove("ConfirmPassword");
            }

            if (ModelState.IsValid)
            {
                var user = await _dashboardService.GetUserByIdAsync(id);
                var role = await _dashboardService.GetUserRolesAsync(user);
                bool isAdmin = role.Contains("Admin");
                if (user == null || isAdmin)
                {
                    return NotFound();
                }

                user.Email = model.Email;
                user.Address = model.Address;
                user.Age = model.Age;

                if (model.ProfileImage != null && model.ProfileImage.FileName.Length > 0)
                {
                    // Delete the old profile image if it exists
                    if (!string.IsNullOrEmpty(user.imageUrl))
                    {
                        string oldImagePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", user.imageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string uploadsFolder = Path.Combine(Environment.CurrentDirectory, "wwwroot", "images", "profiles");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfileImage.CopyToAsync(fileStream);
                    }

                    user.imageUrl = "/images/profiles/" + uniqueFileName;
                }

                var updateResult = await _dashboardService.UpdateUserAsync(user, model.NewPassword);
                if (updateResult.Succeeded)
                {
                    var roleUpdateResult = await _dashboardService.UpdateUserRoleAsync(user, model.RoleId);
                    if (roleUpdateResult.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in roleUpdateResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            ViewBag.RoleList = roleService.getRoles();
            return View(model);
        }



        //[Authorize(Policy = AuthenticationConstants.Identity.Create)]
        public IActionResult Create()
        {
            var roles = roleService.getRoles();
            ViewBag.RoleList = roles;
            return View();
        }

        //[Authorize(Policy = AuthenticationConstants.Identity.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _dashboardService.CreateUserAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewBag.RoleList = roleService.getRoles();
            return View(model);
        }

        public async Task<IActionResult> ManageClaims(string id)
        {

            var user = await _dashboardService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userClaims = await _dashboardService.GetUserClaimsAsync(id); 
            foreach (var claim in userClaims)
            {
                
                Console.WriteLine(claim.Type);
            }

            List<UserClaim> arr = new List<UserClaim>();
            foreach (Claim claim in DataLayer.Entities.Claims.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type,
                    IsSelected = userClaims.Any(c => c.Type == claim.Type)
                };
                arr.Add(userClaim);
            }

            var model = new UserClaimsViewModel
            {
                UserId = user.Id,
                UserName = user.UserName, 
                Claims = arr
            };

            return View(model);
        }

        //[Authorize(Policy = AuthenticationConstants.Claims.Manage)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageClaims(UserClaimsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await _dashboardService.UpdateUserClaimsAsync(model.UserId, model.Claims);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Error updating claims.");
            }

            // Repopulate the ViewBag or other necessary data before returning the view
            return View(model);
        }



    }
}
