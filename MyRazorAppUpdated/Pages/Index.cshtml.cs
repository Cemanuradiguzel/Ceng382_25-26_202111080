using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models; // Adjust according to your namespace
using Newtonsoft.Json;  // For JSON serialization
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace RazorApp.Pages;
using MyRazorApp.Utilities; // For Utils class
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public static List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();

    private void EnsureSeeded()
    {
        if (Classes.Count == 0)
        {
            var rnd = new Random();
            for (int i = 1; i <= 100; i++)
            {
                Classes.Add(new ClassInformationModel {
                    // Id zaten ctor’da artıyor
                    ClassName    = $"Class {i}",
                    StudentCount = rnd.Next(10, 50),
                    Description  = $"Description for class {i}"
                });
            }
        }
    }
    // Properties to bind form data
    [BindProperty]
    public string? ClassName { get; set; }

    [BindProperty]
    public int StudentCount { get; set; } 

    [BindProperty]
    public string? Description { get; set; }

    [BindProperty]
    public int? EditId { get; set; }

    // The method you will use for JSON export logic
    private List<ClassInformationModel> GenerateSampleClasses()
    {
        var classes = new List<ClassInformationModel>();

        // Creating 100 sample class data
        for (int i = 1; i <= 100; i++)
        {
            classes.Add(new ClassInformationModel
            {
                Id = i,
                ClassName = "Class " + i,
                StudentCount = new Random().Next(10, 50),
                Description = "Description for class " + i
            });
        }

        return classes;
    }
    

    // JSON Export method (for downloading JSON file)
    public IActionResult OnPostExportJson(string selectedColumns, string? filter, int pageIndex)
    {
        EnsureSeeded();  // sample veriyi sadece bir kez inject eden metot

        // 1) Filtre uygula
        var query = Classes.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
            query = query.Where(c => c.ClassName!
                .Contains(filter, StringComparison.OrdinalIgnoreCase));

        // 2) Sayfalama (isteğe bağlı)
        const int pageSize = 10;
        var paged = query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        // 3) Sütun seçimi
        if (string.IsNullOrWhiteSpace(selectedColumns))
        {
            selectedColumns = "ClassName,StudentCount,Description";  // Varsayılan kolonlar
        }
        
        var cols = selectedColumns
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var data = paged.Select(c => new
        {
            ClassName    = cols.Contains("ClassName")    ? c.ClassName    : null,
            StudentCount = cols.Contains("StudentCount") ? c.StudentCount : (int?)null,
            Description  = cols.Contains("Description")  ? c.Description  : null
        }).ToList();

        var json = JsonConvert.SerializeObject(
            data,
            Formatting.Indented,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
        );

        return File(Encoding.UTF8.GetBytes(json), "application/json", "export.json");
    }

    public IActionResult OnGet(int pageIndex = 1, string? filter = null)
     {
        var username = HttpContext.Session.GetString("Username");
        var token = HttpContext.Session.GetString("Token");

        var cookieUsername = Request.Cookies["Username"];
        var cookieToken = Request.Cookies["Token"];

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token) ||
            username != cookieUsername || token != cookieToken)
        {
            return RedirectToPage("/Login");
        }

        EnsureSeeded();

         var query = Classes.AsQueryable();
         if (!string.IsNullOrWhiteSpace(filter))
             query = query.Where(c => c.ClassName!
                 .Contains(filter, StringComparison.OrdinalIgnoreCase));

         const int pageSize = 10;
         var paged = query
             .Skip((pageIndex - 1) * pageSize)
             .Take(pageSize)
             .ToList();

         ViewData["ClassList"]   = paged;
         ViewData["CurrentPage"] = pageIndex;
         ViewData["TotalPages"]  = (int)Math.Ceiling(query.Count() / (double)pageSize);

         return Page();
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
            // Use the null-forgiving operator to assure the compiler
            ClassName = classToEdit.ClassName!;
            StudentCount = classToEdit.StudentCount;
            Description = classToEdit.Description!;
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
    public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("Token");
            return RedirectToPage("/Login"); // Redirect to login page
        }
}
