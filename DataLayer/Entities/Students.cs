using DataLayer.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Range(0, 60)]
        public int Age { get; set; }
        [Required]
        [EmailAddress]
        [UniqueEmailForCreate]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public ICollection<CourseStudent>? CourseStudents { get; set; } = new List<CourseStudent>();

    }
}
