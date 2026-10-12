using System.ComponentModel.DataAnnotations;

namespace Grad_Project_ITI_5.Models
{
    public class StudentCourse
    {
        public int StudentCourseId { get; set; }
        public DateTime CourseEnrollmentDate { get; set; } = DateTime.Now;


        //Relations
        public int StudentId { get; set; }
        public Student Student { get; set; } = new Student();
        public int CourseId { get; set; }
        public Course Course { get; set; } = new Course();
    }
}
