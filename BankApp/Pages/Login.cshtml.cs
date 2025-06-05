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
        [BindProperty]
        public string? Login { get; set; }
        
        [BindProperty]
        public string? Password { get; set; }
        
        [TempData]
        public string? GeneratedLogin { get; set; }

        //Generuje losowy 8-znakowy login.
        public void OnGet(string? generatedLogin)
        {
            if (!string.IsNullOrEmpty(generatedLogin))
            {
                GeneratedLogin = $"Twój login to: {generatedLogin}";
            }
        }
        
        // Proces logowania, program szuka w czy w bazie znajduje się podany przez użytkownika login z hasłem odpowiadającym temu, co również podał użytkownik.
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
                    IsPersistent = true,
                    ExpiresUtc   = DateTimeOffset.UtcNow.AddMinutes(30)
                });

            Response.Cookies.Append("SelectedIban", user.Iban!, new CookieOptions
            {
                IsEssential = true,
                Expires     = DateTimeOffset.UtcNow.AddDays(1),
                Secure      = true,
                SameSite    = SameSiteMode.Strict,
                HttpOnly    = false
            });
            
            // Tutaj przechowywane są informacje na aktualną sesję użytkownika, pod warunkiem że logowanie przebiegnie pomyślnie.
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