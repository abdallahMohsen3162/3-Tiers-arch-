using businessLogic.ModelViews;
using businessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        //teacher or admin


        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {

            var model = _dashboardService.GetDashboardData();
            return View(model);
        }

        [Authorize]
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

        [Authorize]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
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


        [Authorize]
        public IActionResult Create()
        {
            var roles = roleService.getRoles();
            ViewBag.RoleList = roles;
            return View();
        }

        [Authorize]
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
    }
}
