using System.ComponentModel.DataAnnotations;

namespace Grad_Project_ITI_5.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }


        //Relations
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
