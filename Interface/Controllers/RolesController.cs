using businessLogic.ModelViews;
using Microsoft.AspNetCore.Mvc;
using businessLogic.Services.Interfaces;
using System.Threading.Tasks;

namespace Interface.Controllers
{
    public class RolesController : Controller
    {
        private readonly IRolesService _roleManager;

        public RolesController(IRolesService roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.RoleList = _roleManager.getRoles();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateRoleAsync(model.RoleName);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            ViewBag.RoleList = _roleManager.getRoles();
            return View("Create", model);
        }

        public IActionResult Index()
        {
            var roles = _roleManager.getRoles();
            ViewBag.RoleList = roles;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.UpdateRoleAsync(model.Id, model.RoleName);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _roleManager.DeleteRoleAsync(id);
            ViewBag.RoleList = _roleManager.getRoles();
            return RedirectToAction(nameof(Index));
        }

    }
}
