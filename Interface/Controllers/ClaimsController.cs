using DataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using businessLogic.ModelViews;

namespace YourNamespace.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ClaimsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /Claims/ManageClaims/{userId}
        [HttpGet]
        [Authorize(Roles = "Admin")]  // Only allow Admins to manage claims
        public async Task<IActionResult> ManageClaims(string userId)
        {

            return View();
        }

        // POST: /Claims/AddClaim
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddClaim(string userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var claim = new Claim(claimType, claimValue);
            var result = await _userManager.AddClaimAsync(user, claim);

            if (!result.Succeeded)
            {
                // Handle errors
                ModelState.AddModelError("", "Failed to add claim.");
            }

            return RedirectToAction("ManageClaims", new { userId = userId });
        }

        // POST: /Claims/RemoveClaim
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveClaim(string userId, string claimType)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var claimToRemove = claims.FirstOrDefault(c => c.Type == claimType);
            if (claimToRemove == null)
            {
                return NotFound();
            }

            var result = await _userManager.RemoveClaimAsync(user, claimToRemove);
            if (!result.Succeeded)
            {
                // Handle errors
                ModelState.AddModelError("", "Failed to remove claim.");
            }

            return RedirectToAction("ManageClaims", new { userId = userId });
        }
    }
}
