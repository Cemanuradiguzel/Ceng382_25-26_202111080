using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models; // Adjust according to your namespace
using System.Collections.Generic;
using System.Linq;
namespace RazorApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public static List<ClassInformationModel> Classes = new List<ClassInformationModel>();

    // Properties to bind form data
    [BindProperty]
    public string ClassName { get; set; }

    [BindProperty]
    public int StudentCount { get; set; }

    [BindProperty]
    public string Description { get; set; }

    [BindProperty]
    public int? EditId { get; set; }

    public void OnGet()
    {
        // Fetch all classes from the static class data
        Classes = ClassData.Classes;
    }

    public IActionResult OnPostAddOrEdit()
    {
        if (string.IsNullOrWhiteSpace(ClassName) || StudentCount <= 0 || string.IsNullOrWhiteSpace(Description))
        {
            ModelState.AddModelError("", "All fields are required and Student Count must be positive.");
            return Page();
        }

        if (EditId.HasValue)
        {
            // Güncelleme işlemi
            var classToEdit = Classes.FirstOrDefault(c => c.Id == EditId.Value);
            if (classToEdit != null)
            {
                classToEdit.ClassName = ClassName;
                classToEdit.StudentCount = StudentCount;
                classToEdit.Description = Description;
            }
        }
        else
        {
                // Yeni ekleme işlemi
            int newId = Classes.Count > 0 ? Classes.Max(c => c.Id) + 1 : 1;
            Classes.Add(new ClassInformationModel
            {
                Id = newId,
                ClassName = ClassName,
                StudentCount = StudentCount,
                Description = Description
            });
        }

        // Formu temizle
        ClassName = string.Empty;
        StudentCount = 0;
        Description = string.Empty;
        EditId = null;

        return RedirectToPage();
    }

    public IActionResult OnPostEdit(int id)
    {
        var classToEdit = Classes.FirstOrDefault(c => c.Id == id);
        if (classToEdit != null)
        {
            ClassName = classToEdit.ClassName;
            StudentCount = classToEdit.StudentCount;
            Description = classToEdit.Description;
            EditId = id; // Düzenleme için Id atandı
        }
        return Page();
    }
    public IActionResult OnPostDelete(int id)
    {
        // Find and remove the class by Id
        var classToDelete = Classes.FirstOrDefault(c => c.Id == id);
        if (classToDelete != null)
        {
            Classes.Remove(classToDelete);
        }

        // Redirect to refresh the page after deletion
        return RedirectToPage();
    }
}
