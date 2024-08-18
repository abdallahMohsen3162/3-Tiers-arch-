using Microsoft.AspNetCore.Mvc;
using businessLogic.Services.Interfaces;
using DataLayer.Entities;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

public class CoursesController : Controller
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        foreach (var course in courses)
        {
            Console.WriteLine(course.Name);
        }
        return View(courses);
    }


    public IActionResult Create()
    {
        ViewBag.CourseStates = _courseService.GetCourseStatesSelectList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (ModelState.IsValid)
        {
            await _courseService.AddCourseAsync(course);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.CourseStates = _courseService.GetCourseStatesSelectList(course.State);
        return View(course);
    }

    public async Task<IActionResult> Delete(int id)
    {
        await _courseService.MarkCourseAsDeletedAsync(id);
        return RedirectToAction(nameof(Index));
    }

 
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        ViewBag.CourseStates = _courseService.GetCourseStatesSelectList(course.State);
        return View(course);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course course)
    {
        if (ModelState.IsValid)
        {
            await _courseService.UpdateCourseAsync(course);
            return RedirectToAction(nameof(Index));
        }
        return View(course);
    }
}