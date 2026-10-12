using System.ComponentModel.DataAnnotations;

namespace Grad_Project_ITI_5.Models
{
    public class Course
    {
        
        public int CourseId { get; set; }
        [Required]
        [StringLength(100)]
        public string? Title { get; set; }
        [Required]
        public int Credits { get; set; }


        //Relations
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; } = new Instructor();
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = new Department();
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
