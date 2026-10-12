using System.ComponentModel.DataAnnotations;

namespace Grad_Project_ITI_5.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        //Relations

        public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
