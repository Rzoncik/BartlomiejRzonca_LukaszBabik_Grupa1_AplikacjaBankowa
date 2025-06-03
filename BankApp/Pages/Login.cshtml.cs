using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BankApp.Models;
using BankApp.Helpers;

namespace BankApp.Pages
{
    public class LoginModel(AppDbContext context) : PageModel
    {
        // [BindProperty]
        // private int UserId { get; set; }
        //
        [BindProperty]
        // [Required(ErrorMessage = "Wprowadź login.")]
        public string? Login { get; set; }
        
        [BindProperty]
        // [Required(ErrorMessage = "Wprowadź hasło.")]
        public string? Password { get; set; }

        // [Required]
        // public string? Message { get; set; }
        
        [TempData]
        public string? GeneratedLogin { get; set; }
        
        // [BindProperty]
        // private string? FirstName { get; set; }

        //Generuje losowy 8-znakowy login.
        public void OnGet(string? generatedLogin)
        {
            if (!string.IsNullOrEmpty(generatedLogin))
            {
                GeneratedLogin = $"Twój login to: {generatedLogin}";
            }
        }
        
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (Login == null || Password == null)
            {
                ModelState.AddModelError(string.Empty, "Login i hasło są wymagane.");
                return Page();
            }

            var hashedInput = PasswordHelper.HashPassword(Password!);
            
            var user = context.Users
                .FirstOrDefault(u => u.Login == Login && u.Password == hashedInput);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Niepoprawny login lub hasło.");
                return Page();
            }

            // ——— build the identity that will be stored in the cookie ———
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Login!),
            };

            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = true,                 // “Remember me” style; make false for session‑only
                    ExpiresUtc   = DateTimeOffset.UtcNow.AddMinutes(30)
                });

            Response.Cookies.Append("SelectedIban", user.Iban!, new CookieOptions
            {
                IsEssential = true,                  // always stored even with cookie-consent banners
                Expires     = DateTimeOffset.UtcNow.AddDays(1),
                Secure      = true,                  // only over HTTPS
                SameSite    = SameSiteMode.Strict,   // prevents cross-site sending
                HttpOnly    = false                  // you need JS to read it on Dashboard
            });
            
            // Example of storing an extra per‑user value in server‑side session (optional):
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Password", user.Password);
            HttpContext.Session.SetString("PasswordConfirm", user.PasswordConfirm!);
            HttpContext.Session.SetString("Balance", user.Balance.ToString(CultureInfo.InvariantCulture));
            HttpContext.Session.SetString("Iban", user.Iban!);
            HttpContext.Session.SetString("CreditCardNumber", user.CreditCardNumber!);
            HttpContext.Session.SetString("CreditCardExpiry", user.CreditCardExpiry!);
            HttpContext.Session.SetString("CreditCardCvv", user.CreditCardCvv!);
            HttpContext.Session.SetString("FirstName", user.FirstName);
            HttpContext.Session.SetString("LastName", user.LastName);
            
            return RedirectToPage("/Dashboard");
        }
    }
}