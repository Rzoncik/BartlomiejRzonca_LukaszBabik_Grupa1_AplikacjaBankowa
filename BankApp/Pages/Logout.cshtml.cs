using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages
{
    public class Logout : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            // Usuwa wszystko z sesji użytkownika.
            HttpContext.Session.Clear();

            // Usuwa ciasteczka.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Przenosi użytkownika na stronę główną.
            return RedirectToPage("/Index");
        }
    }
}