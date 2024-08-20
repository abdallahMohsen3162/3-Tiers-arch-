using Microsoft.AspNetCore.Mvc;

namespace Interface.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult AccessDenied()
        {
            Console.WriteLine("access denied");
            return View();
        }
    }
}
