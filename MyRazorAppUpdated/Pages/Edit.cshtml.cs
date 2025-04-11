using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using MyRazorApp.Models;

namespace MyRazorApp.Pages
{
    public class EditModel : PageModel
    {
        // Define a property to hold the class information
        [BindProperty]
        public ClassInformationModel? ClassInfo { get; set; }

        [BindProperty]
        public string? ClassName { get; set; }

        [BindProperty]
        public int StudentCount { get; set; }

        [BindProperty]
        public string? Description { get; set; }

        // This method is used to populate the form when the page loads (GET request)
        public IActionResult OnGet(int id)
        {
            // Find the class by Id from the static list in ClassData
            ClassInfo = ClassData.Classes.FirstOrDefault(c => c.Id == id);

            if (ClassInfo == null)
            {
                return NotFound(); // Return a 404 if the class is not found
            }

            return Page();
        }

        // This method handles the form submission (POST request) when saving the changes
        public IActionResult OnPost(int id)
        {
            // Find the class by Id in the static list in ClassData
            var classToEdit = ClassData.Classes.FirstOrDefault(c => c.Id == id);
            if (classToEdit == null || ClassInfo == null)
            {
                return NotFound();
            }

            // Update the class properties with the new form data
            classToEdit.ClassName = ClassInfo.ClassName ?? classToEdit.ClassName;
            classToEdit.StudentCount = ClassInfo.StudentCount;
            classToEdit.Description = ClassInfo.Description ?? classToEdit.Description;

            // Redirect back to the Index page to see the updated list
            return RedirectToPage("/Index");
        }
    }
}
