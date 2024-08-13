using DataLayer;
using DataLayer.Data;
using DataLayer.Entities;
using System.Collections.Generic;
using System.Linq;
using businessLogic.Repositories;

namespace businessLogic.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Course> GetCourses()
        {
            return _context.Courses.ToList();
        }

    }
}
