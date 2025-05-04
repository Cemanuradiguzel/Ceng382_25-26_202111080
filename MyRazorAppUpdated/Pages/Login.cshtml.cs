using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorAppUpdated.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Linq;
using MyRazorApp.Models;

namespace RazorApp.Pages
{
    /*[Authorize(Roles = "admin")] // Only admin can access this page
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        // Bind properties for filtering and pagination
        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; } // Filter for class name
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1; // Default page index is 1

        public List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>(); // To hold the list of classes
        public int TotalPages { get; set; }

        // Define a collection to hold the classes
        private static List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();

        public void OnGet()
        {
            EnsureSeeded(); // Make sure the sample data is available

            var query = Classes.AsQueryable(); // Get the classes as an IQueryable

            // Apply filter if provided
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                query = query.Where(c => c.ClassName.Contains(Filter ?? string.Empty, StringComparison.OrdinalIgnoreCase));
            }

            const int pageSize = 10; // Define the page size
            var paged = query
                .Skip((PageIndex - 1) * pageSize) // Skip previous pages
                .Take(pageSize) // Take only the number of items for the current page
                .ToList(); // Execute the query and convert it to a list

            ClassList = paged; // Assign to the property that will be accessed in the view
            TotalPages = (int)Math.Ceiling(query.Count() / (double)pageSize); // Calculate total pages based on the total count

            // Optionally, you can log the result for debugging purposes
            _logger.LogInformation($"Displaying page {PageIndex} of {TotalPages}");
        }

        private void EnsureSeeded()
        {
            // Ensure there's sample data if the list is empty
            if (!Classes.Any())
            {
                var rnd = new Random();
                for (int i = 1; i <= 100; i++)
                {
                    Classes.Add(new ClassInformationModel
                    {
                        ClassName = $"Class {i}",
                        StudentCount = rnd.Next(10, 50),
                        Description = $"Description for class {i}"
                    });
                }
            }
        }
    }*/
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string? Username { get; set; }
        [BindProperty]
        public string? Password { get; set; }

        // Kullanıcıları tutmak için geçici bir liste (gerçek projelerde veritabanı kullanılacak)
        public static List<User> Users = RegisterModel.Users;  // Kullanıcıları önceden kaydettik

        /*public IActionResult OnPost()
        {
            // Kullanıcıyı bul
            var user = Users.FirstOrDefault(u => u.Username == Username);

            // Kullanıcı var mı ve şifre doğru mu?
            if (user != null && user.Password == Password)  // Burada şifreyi hash'leyip kontrol etmelisiniz
            {
                // Giriş başarılı, admin ise admin sayfasına yönlendir
                if (user.Role == "admin")
                {
                    return RedirectToPage("/AdminPage");
                }
                
                // Normal kullanıcı ise ana sayfaya yönlendir
                return RedirectToPage("/Index");
            }

            // Giriş başarısızsa hata mesajı
            ModelState.AddModelError("", "Invalid Username or Password");
            return Page();
        }
        public IActionResult OnPost()
        {
            var users = GetUsersFromJson(); // Get the users from the JSON file
            var user = users.FirstOrDefault(u => u.Username == Username && u.Password == Password);

            if (user != null && user.isActive)
            {
                // Create token and store session
                HttpContext.Session.SetString("Username", Username ?? string.Empty);
                HttpContext.Session.SetString("Token", GenerateToken());
                // Save session and cookies
                Response.Cookies.Append("Username", Username ?? string.Empty, new CookieOptions 
                { 
                    Expires = DateTime.Now.AddMinutes(30), 
                    HttpOnly = true 
                });

                // Check if the user is an admin and redirect accordingly
                if (user.Role == "admin")
                {
                    return RedirectToPage("/AdminPage"); // Redirect admin users to the AdminPage
                }

                return RedirectToPage("/Index"); // Redirect other users to the main page
            }
            else
            {
                ModelState.AddModelError("", "Invalid Username or Password");
                return Page();
            }
        }*/
        public IActionResult OnPost(string? returnUrl = null)
        {
            var users = GetUsersFromJson();
            var user = users.FirstOrDefault(u => u.Username == Username && u.Password == Password);

            if (user != null && user.isActive)
            {
                var sessionId = HttpContext.Session.Id;
                var token = GenerateToken();

                HttpContext.Session.SetString("Username", Username ?? string.Empty);
                HttpContext.Session.SetString("Token", token);
                HttpContext.Session.SetString("SessionId", sessionId);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };

                Response.Cookies.Append("Username", Username ?? string.Empty, cookieOptions);
                Response.Cookies.Append("Token", token, cookieOptions);
                Response.Cookies.Append("SessionId", sessionId, cookieOptions);

                // Eğer ReturnUrl varsa oraya git
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Yoksa Index'e git
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError("", "Invalid Username or Password");
            return Page();
        }

        private string GenerateToken()
        {
            // Token generation logic (for example, JWT or any other method)
            return Guid.NewGuid().ToString(); // Simple token for demonstration
        }
        private List<User> GetUsersFromJson()
        {
            // Read the JSON file and deserialize it to a list of users
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");
            var json = System.IO.File.ReadAllText(path);

            return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
        }
        public IActionResult OnGet()
        {
            return Page();
        }
        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("Token");
            return RedirectToPage("/Login"); // Redirect to login page
        }
    }
}
