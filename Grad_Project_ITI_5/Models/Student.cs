using System.ComponentModel.DataAnnotations;
using System.Security;

namespace Grad_Project_ITI_5.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
    
        public string? Email { get; set; }
        [Required]
        [Phone(ErrorMessage = "Enter a valid phone number")]
        public string? PhoneNumber { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public String? ProfileImage { get; set; }

        //Relations
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

    }
}
