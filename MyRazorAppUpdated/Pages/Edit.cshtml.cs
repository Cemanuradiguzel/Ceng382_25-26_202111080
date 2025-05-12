using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using RazorApp.Data;
using RazorApp.Models;
using System.Threading.Tasks;

namespace RazorApp.Pages
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Define a property to hold the class information
        [BindProperty]
        public Class? ClassInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var classEntity = _context.Classes == null ? null : await _context.Classes.FindAsync(id);
            if (classEntity == null)
            {
                return NotFound();
            }
            ClassInfo = classEntity;

            if (ClassInfo == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.Classes == null)
            {
                return NotFound();
            }
            var classToUpdate = await _context.Classes.FindAsync(id);

            if (classToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Class>(
                classToUpdate,
                "ClassInfo", // Prefix for form fields
                c => c.Name, c => c.PersonCount, c => c.Description))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}