using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models; // Adjust according to your namespace
using Newtonsoft.Json;  // For JSON serialization
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace RazorApp.Pages;
using MyRazorApp.Utilities; // For Utils class

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
   /* public IActionResult OnPostExportJson(string[] selectedColumns)
    {
        var classes = GenerateSampleClasses();  // Example data (replace with actual data)

        // If no columns selected, export all columns
        if (selectedColumns.Length == 0)
        {
            selectedColumns = new[] { "ClassName", "StudentCount", "Description" }; // Default all columns
        }

        // Create a filtered list based on selected columns
        var filteredData = classes.Select(c => new
        {
            ClassName = selectedColumns.Contains("ClassName") ? c.ClassName : null,
            StudentCount = selectedColumns.Contains("StudentCount") ? c.StudentCount : 0,
            Description = selectedColumns.Contains("Description") ? c.Description : null
        }).ToList();

        // Serialize the filtered data to JSON
        var jsonResult = JsonConvert.SerializeObject(filteredData);

        // Return the JSON file for download
        return File(Encoding.UTF8.GetBytes(jsonResult), "application/json", "export.json");
    }
public IActionResult OnPostExportJson(string selectedColumns)
{
    if (string.IsNullOrEmpty(selectedColumns))
    {
        // If no columns are selected, export all columns
        selectedColumns = "ClassName,StudentCount,Description";
    }
    // Split the selected columns by commas
    var selectedColumnArray = selectedColumns.Split(',');

    // Fetch the paginated class data from ViewData
    var classes = ViewData["ClassList"] as List<ClassInformationModel>;

    if (classes == null)
    {
        // If ViewData["ClassList"] is null, return a bad request or appropriate message
        return BadRequest("No class data available to export.");
    }
    
    // Filter the data based on selected columns
    var filteredData = classes.Select(c => new
    {
        ClassName = selectedColumnArray.Contains("ClassName") ? c.ClassName : null,
        StudentCount = selectedColumnArray.Contains("StudentCount") ? c.StudentCount : 0,
        Description = selectedColumnArray.Contains("Description") ? c.Description : null
    }).ToList();

    // Serialize the filtered data to JSON
    var jsonResult = JsonConvert.SerializeObject(filteredData);

    // Return the JSON file for download
    return File(Encoding.UTF8.GetBytes(jsonResult), "application/json", "export.json");
}*/
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
}
