using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorAppUpdated.Models; // Adjust according to your namespace
using Newtonsoft.Json;  // For JSON serialization

namespace RazorApp.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string? Username { get; set; }
        [BindProperty]
        public string? Password { get; set; }
        [BindProperty]
        public string? Role { get; set; }

        // Kullanıcıları tutmak için geçici bir liste (gerçek projelerde veritabanı kullanılacak)
        public static List<User> Users = new List<User>();

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var newUser = new User
            {
                Username = Username,
                Password = Password,  // Hash the password here in a real app
                Role = Role,
                isActive = true,
                CreatedAt = DateTime.Now
            };

            // Redirect to login page after registration
            return RedirectToPage("/Login");
        }

    }
}
