using Grad_Project_ITI_5.Data;
using Grad_Project_ITI_5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

public class StudentsController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public StudentsController(
        AppDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }   
    
    
    private async Task LoadDepartments()
    {
        ViewBag.DepartmentId = new SelectList(
            await _context.Departments.ToListAsync(),
            "DepartmentId",
            "Name"
        );
    }

    private IQueryable<Student> FilterByDepartment(IQueryable<Student> students, int? departmentId)
    {
        if (departmentId.HasValue)
            students = students.Where(s => s.DepartmentId == departmentId);

        return students;
    }
    private IQueryable<Student> FilterByDate(IQueryable<Student> students, DateTime? date)
    {
        if (date.HasValue)
        {
            students = students.Where(s =>
                s.EnrollmentDate >= date.Value.Date &&
                s.EnrollmentDate < date.Value.Date.AddDays(1));
        }

        return students;
    }
    private IQueryable<Student> Search(IQueryable<Student> students, string? search)
    {
        if (!string.IsNullOrEmpty(search))
            students = students.Where(s =>
                s.FullName.Contains(search)) ;

        return students;
    }
    public async Task<IActionResult> Index(string? search,int? departmentId,DateTime? date)
    {
        var students = _context.Students
            .Include(s => s.Department)
            .Include(s => s.StudentCourses)
            .ThenInclude(sc => sc.Course)
            .AsQueryable();

        students = Search(students, search);
        students = FilterByDepartment(students, departmentId);
        students = FilterByDate(students, date);

        ViewBag.Departments = new SelectList(
            await _context.Departments.ToListAsync(),
            "DepartmentId",
            "Name",
            departmentId);

        ViewBag.Search = search;
        ViewBag.EnrollmentDate = date?.ToString("yyyy-MM-dd");

        return View(await students.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students.Include(s => s.StudentCourses)
            .ThenInclude(sc => sc.Course)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    public async Task<IActionResult> Create()
    {
        await LoadDepartments();
        return View();
    }


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("FullName,Email,PhoneNumber,DepartmentId")] Student student,IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartments();
            return View(student);
        }

        student.EnrollmentDate = DateTime.Now;

        try
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "imageFile",
                        "Image size must not exceed 5 MB.");

                    await LoadDepartments();
                    return View(student);
                }

                var extension = Path.GetExtension(imageFile.FileName);

                var allowedExtensions = new[]
                {
                ".jpg", ".jpeg", ".png", ".webp"
            };

                if (!allowedExtensions.Contains(
                    extension.ToLowerInvariant()))
                {
                    ModelState.AddModelError(
                        "imageFile",
                        "Please upload a JPG, JPEG, PNG, or WEBP image.");

                    await LoadDepartments();
                    return View(student);
                }

                var folderPath = Path.Combine(
                    _environment.WebRootPath,
                    "images");

                Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(
                    filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                student.ProfileImage = "/images/" + fileName;
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.GetBaseException().Message);

            await LoadDepartments();
            return View(student);
        }
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students.FindAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        await LoadDepartments();

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,[Bind("StudentId,FullName,Email,PhoneNumber,DepartmentId")] Student student,IFormFile? imageFile)
    {
        if (id != student.StudentId)
        {
            return NotFound();
        }

        var existingStudent = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (existingStudent == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            student.ProfileImage = existingStudent.ProfileImage;

            await LoadDepartments();
            return View(student);
        }

        existingStudent.FullName = student.FullName;
        existingStudent.Email = student.Email;
        existingStudent.PhoneNumber = student.PhoneNumber;
        existingStudent.DepartmentId = student.DepartmentId;

        if (imageFile != null && imageFile.Length > 0)
        {
            if (imageFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(
                    "imageFile",
                    "Image size must not exceed 5 MB.");

                student.ProfileImage = existingStudent.ProfileImage;

                await LoadDepartments();
                return View(student);
            }

            var extension = Path.GetExtension(imageFile.FileName);

            var allowedExtensions = new[]
            {
                ".jpg", ".jpeg", ".png", ".webp"
            };

            if (!allowedExtensions.Contains(
                extension.ToLowerInvariant()))
            {
                ModelState.AddModelError(
                    "imageFile",
                    "Please upload a JPG, JPEG, PNG, or WEBP image.");

                student.ProfileImage = existingStudent.ProfileImage;

                await LoadDepartments();
                return View(student);
            }

            var folderPath = Path.Combine(
                _environment.WebRootPath,
                "images");

            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(
                filePath,
                FileMode.Create);

            await imageFile.CopyToAsync(stream);

            existingStudent.ProfileImage = "/images/" + fileName;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StudentExists(student.StudentId))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students.FindAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        var imagePath = student.ProfileImage;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(imagePath) &&
            imagePath.StartsWith(
                "/images/",
                StringComparison.OrdinalIgnoreCase))
        {
            var fileName = Path.GetFileName(imagePath);

            var fullPath = Path.Combine(
                _environment.WebRootPath,
                "images",
                fileName);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        return RedirectToAction(nameof(Index));
    }

    private bool StudentExists(int id)
    {
        return _context.Students.Any(
            s => s.StudentId == id);
    }
}
