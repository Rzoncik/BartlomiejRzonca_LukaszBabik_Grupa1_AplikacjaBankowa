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
            // 1️⃣  remove everything stored in the server-side session
            HttpContext.Session.Clear();

            // 2️⃣  delete the authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 3️⃣  bounce the user back to Login (or Index – your choice)
            return RedirectToPage("/Index");
        }
    }
}