using businessLogic.ModelViews;
using businessLogic.Services.Services;
using DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using businessLogic.Services.Interfaces;

namespace Interface.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private int segment;
        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
            this.segment = 3;
        }

 
        public IActionResult Index(int page = 1)
        {
            Console.WriteLine("page: 123123132132132");
            int totalStudents = _studentService.GetTotalStudents();
            int totalPages = (int)Math.Ceiling(totalStudents / (this.segment * 1.0));

            var model = _studentService.GetStudentsForPage(page);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(model);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentService.GetStudentById(id.Value);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                await _studentService.AddStudent(student);
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentService.FindStudentById(id.Value);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _studentService.UpdateStudent(student);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_studentService.StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentService.GetStudentById(id.Value);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _studentService.FindStudentById(id);
            await _studentService.RemoveStudent(student);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Courses(int id)
        {
            var student = await _studentService.GetStudentWithCourses(id);

            if (student == null)
            {
                return NotFound();
            }

            var allCourses = await _studentService.GetAvailableCourses();
            var takenCourses = student.CourseStudents.Select(cs => cs.Course).ToList();
            var notTakenCourses = allCourses.Except(takenCourses).ToList();
            foreach (var course in allCourses)
            {
                Console.WriteLine(course.Name);
            }
            var viewModel = new studentViewModel
            {
                student = student,
                taken = takenCourses,
                notTaken = notTakenCourses
            };

            return View(viewModel);
        }

        [HttpPost]

        public async Task<IActionResult> EnrollInCourses(int studentId, int[] courseIds)
        {
            var student = await _studentService.GetStudentWithCourses(studentId);

            if (student == null)
            {
                return NotFound();
            }

            await _studentService.EnrollStudentInCourses(student, courseIds);
            return RedirectToAction("Courses", new { id = studentId });
        }

        [HttpPost]

        public async Task<IActionResult> DeleteCourses(int studentId, int[] courseIdsToRemove)
        {
            var student = await _studentService.GetStudentWithCourses(studentId);

            if (student == null)
            {
                return NotFound();
            }

            var coursesToRemove = await _studentService.GetEnrolledCourses(studentId, courseIdsToRemove);
            await _studentService.DeleteCourses(student, coursesToRemove);
            return RedirectToAction("Courses", new { id = studentId });
        }
    }
}
