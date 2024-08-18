using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using businessLogic.Services.Interfaces;
using DataLayer.Data;
using DataLayer.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace businessLogic.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses.OrderBy(c => c.Sort).ToListAsync();
        }

        public SelectList GetCourseStatesSelectList(CourseState? selectedState = null)
        {
            return new SelectList(Enum.GetValues(typeof(CourseState))
                .Cast<CourseState>()
                .Select(s => new { Value = s, Text = s.ToString() }), "Value", "Text", selectedState);
        }

        public async Task<Course> GetCourseByIdAsync(int id)
        {
            return await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddCourseAsync(Course course)
        {
            _context.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _context.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task MarkCourseAsDeletedAsync(int id)
        {
            var course = await GetCourseByIdAsync(id);
            if (course != null)
            {
                course.State = CourseState.Deleted;
                await _context.SaveChangesAsync();
            }
        }
    }
}
