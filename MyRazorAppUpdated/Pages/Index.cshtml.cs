using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;  // For JSON serialization
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
namespace RazorApp.Pages;
using RazorApp.Utilities; // For Utils class
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RazorApp.Data;
using RazorApp.Models;
using Microsoft.EntityFrameworkCore;

    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(SchoolDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<Class> ClassList { get; set; } = new List<Class>();

        [BindProperty]
        public string? Name { get; set; }

        [BindProperty]
        public int PersonCount { get; set; }

        [BindProperty]
        public string? Description { get; set; }

        [BindProperty]
        public int? EditId { get; set; }

        private void EnsureSeeded()
        {
            if (!_context.Classes.Any())
            {
                var rnd = new Random();
                for (int i = 1; i <= 100; i++)
                {
                    _context.Classes.Add(new Class
                    {
                        Name = $"Class {i}",
                        PersonCount = rnd.Next(10, 50), // 10 ile 50 arasında rastgele sayı
                        Description = $"Description for class {i}",
                        IsActive = true
                    });
                }
                _context.SaveChanges();
            }
        }
        public async Task<IActionResult> OnGetAsync(int pageIndex = 1, string? filter = null)
        {
            // Authentication check
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            // Ensure seeded data
            EnsureSeeded();

            // Database query with filtering
            IQueryable<Class> query = _context.Classes;
            
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(c => c.Name != null && c.Name.Contains(filter));
            }

            // Pagination
            const int pageSize = 10;
            ClassList = await query
                .OrderBy(c => c.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = pageIndex;
            ViewData["TotalPages"] = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);
            return Page();
        }

        public async Task<IActionResult> OnPostAddOrEdit()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (EditId.HasValue)
            {
                // Update existing
                var classToEdit = await _context.Classes.FindAsync(EditId.Value);
                if (classToEdit != null)
                {
                    classToEdit.Name = Name;
                    classToEdit.PersonCount = PersonCount;
                    classToEdit.Description = Description;
                }
            }
            else
            {
                // Add new
                _context.Classes.Add(new Class
                {
                    Name = Name,
                    PersonCount = PersonCount,
                    Description = Description,
                    IsActive = true
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEdit(int id)
        {
            var classToEdit = await _context.Classes.FindAsync(id);
            if (classToEdit != null)
            {
                Name = classToEdit.Name;
                PersonCount = classToEdit.PersonCount;
                Description = classToEdit.Description;
                EditId = id;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var classToDelete = await _context.Classes.FindAsync(id);
            if (classToDelete != null)
            {
                _context.Classes.Remove(classToDelete);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("Token");
            return RedirectToPage("/Login");
        }

        private bool IsUserAuthenticated()
        {
            var username = HttpContext.Session.GetString("Username");
            var token = HttpContext.Session.GetString("Token");
            var cookieUsername = Request.Cookies["Username"];
            var cookieToken = Request.Cookies["Token"];

            return !string.IsNullOrEmpty(username) && 
                   !string.IsNullOrEmpty(token) &&
                   username == cookieUsername && 
                   token == cookieToken;
        }
    }
