using Microsoft.AspNetCore.Mvc;

namespace Interface.Controllers
{
    public class CoursesController : Controller
    {
        public CoursesController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
