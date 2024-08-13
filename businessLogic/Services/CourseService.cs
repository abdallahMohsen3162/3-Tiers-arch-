using DataLayer;
using businessLogic.Repositories;
using businessLogic.Services;
using DataLayer.Data;
using DataLayer.Entities;




namespace businessLogic.Services
{
    public class CourseService: ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository _courseRepository)
        {
            this._courseRepository = _courseRepository;
        }

        public List<Course> GetCourses()
        {
            return _courseRepository.GetCourses();
        }

    }
}
