using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorApp.Pages
{
    [Authorize(Roles = "admin")] // Sadece admin rolü olan kullanıcılar erişebilir
    public class AdminPageModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Admin girişi sonrası Index sayfasına yönlendir
            return RedirectToPage("/Index");
        }
    }
}
