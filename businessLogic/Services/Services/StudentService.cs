using businessLogic.Services.Interfaces;
using DataLayer.Data;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace businessLogic.Services.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;
        public readonly int _segment = 3;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // IStudentService implementation
        public int GetTotalStudents()
        {
            return _context.Students.Count();
        }

        public List<Student> GetStudentsForPage(int page)
        {
            int skip = page * _segment - _segment;
            return _context.Students.Skip(skip).Take(_segment).ToList();
        }

        public async Task<Student> GetStudentById(int id)
        {
            return await _context.Students.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddStudent(Student student)
        {
            _context.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task<Student> FindStudentById(int id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task UpdateStudent(Student student)
        {
            _context.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveStudent(Student student)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        public bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        public async Task<Student> GetStudentWithCourses(int id)
        {
            return await _context.Students
                .Include(s => s.CourseStudents)
                .ThenInclude(cs => cs.Course)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Course>> GetAvailableCourses()
        {
            return await _context.Courses.Where(c => c.State != CourseState.Deleted).ToListAsync();
        }

        public async Task<List<CourseStudent>> GetEnrolledCourses(int studentId, int[] courseIds)
        {
            return await _context.CourseStudents
                .Where(cs => courseIds.Contains(cs.CourseId) && cs.StudentId == studentId)
                .ToListAsync();
        }

        public async Task EnrollStudentInCourses(Student student, int[] courseIds)
        {
            var courses = await _context.Courses
                .Where(c => courseIds.Contains(c.Id))
                .ToListAsync();

            foreach (var course in courses)
            {
                if (!student.CourseStudents.Any(cs => cs.CourseId == course.Id))
                {
                    student.CourseStudents.Add(new CourseStudent { CourseId = course.Id, StudentId = student.Id });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourses(Student student, List<CourseStudent> coursesToRemove)
        {
            _context.CourseStudents.RemoveRange(coursesToRemove);
            await _context.SaveChangesAsync();
        }
    }
}
