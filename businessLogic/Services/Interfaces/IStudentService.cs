using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace businessLogic.Services.Interfaces
{
    public interface IStudentService
    {
        int GetTotalStudents();
        List<Student> GetStudentsForPage(int page);
        Task<Student> GetStudentById(int id);
        Task AddStudent(Student student);
        Task<Student> FindStudentById(int id);
        Task UpdateStudent(Student student);
        Task RemoveStudent(Student student);
        bool StudentExists(int id);
        Task<Student> GetStudentWithCourses(int id);
        Task<List<Course>> GetAvailableCourses();
        Task<List<CourseStudent>> GetEnrolledCourses(int studentId, int[] courseIds);
        Task EnrollStudentInCourses(Student student, int[] courseIds);
        Task DeleteCourses(Student student, List<CourseStudent> coursesToRemove);
    }
}
