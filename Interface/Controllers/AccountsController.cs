using businessLogic.ModelViews;
using businessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interface.Controllers
{
    public class AccountsController : Controller
    {

        private IAccountService _accountService;
        public AccountsController(IAccountService accountService)
        {
            this._accountService = accountService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _accountService.GetUserAsync(User);
            var model = _accountService.MapUserToProfileViewModel(user);
            return View(model);
        }

        public IActionResult Index()
        {
            var users = _accountService.GetAllUsers();
            return View(users);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Register")]
        public async Task<IActionResult> RegisterConfirmed(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.RegisterUser(model);
                if (result.Succeeded)
                {
                    var user = await _accountService.GetUserByEmailAsync(model.Email);
                    if (user != null)
                    {
                        await _accountService.SignInUserAsync(user, false);
                    }
                    return RedirectToAction("Index", "Home");
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


        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public IActionResult Login()
        {
            if (_accountService.IsUserAuthenticated(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginConfirmed(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.SignInWithEmailAndPasswordAsync(model.Email, model.Password, model.RememberMe);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account is locked out.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
            return View("Login", model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _accountService.SignOutUserAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerformDelete(DeleteAccountViewModel model)
        {
            if (!ModelState.IsValid) return View("Delete", model);
            var user = await _accountService.GetUserAsync(User);
            bool passwordCheck = await _accountService.CheckPasswordAsync(user, model.Password);
            if (!passwordCheck)
            {
                ModelState.AddModelError(string.Empty, "Incorrect password.");
                return View("Delete", model);
            }
            await _accountService.SignOutUserAsync();
            var result = await _accountService.DeleteUserAsync(user);
            return RedirectToAction("Index", "Home");
        }
    }
}
