using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BankApp.Models;
using BankApp.Helpers;

namespace BankApp.Pages
{
    public class RegisterModel(AppDbContext context) : PageModel
    {
        // Generuje losowy 8-znakowy login.
        private static string GenerateLogin()
        {
            return RandomizeHelper.Random.Next(10000000, 99999999).ToString();
        }

        [BindProperty]
        public new DbUsers User { get; set; } = new();

        // public string Message { get; set; } = string.Empty;

        public IActionResult OnPost()
        {
            // Jeśli są jakiekolwiek błędy, odświeża ponownie stronę.
            if (!ModelState.IsValid)
                return Page();
            
            // Porównuje pola hasła i potwierdzenia hasła i sprawdza, czy są takie same. Jeśli nie są, odświeża stronę i wyświetla komunikat o błędzie.
            if (User.Password != User.PasswordConfirm)
            {
                ModelState.AddModelError(string.Empty, "Hasła nie są takie same.");
                return Page();
            }
            
            // Tutaj wykorzystywane są metody z /Helpers/StringHelper.
            User.FirstName = StringHelper.CapitalizeFirstChar(User.FirstName);
            User.LastName = StringHelper.CapitalizeFirstChar(User.LastName);
            User.City = StringHelper.CapitalizeFirstChar(User.City);
            User.Nationality = StringHelper.CapitalizeFirstChar(User.Nationality);
            User.StreetAndBuilding = StringHelper.CapitalizeFirstChar(User.StreetAndBuilding);
            User.IdCard = StringHelper.CapitalizeFirstThree(User.IdCard);
            
            string newLogin;
            do
            {
                newLogin = GenerateLogin();
            }
            // Sprawdzamy, czy nowo utworzony login już istnieje w bazie danych.
            while (context.Users.Any(u => u.Login == newLogin));
            
            User.Login = newLogin;
            User.Iban = StringHelper.GenerateIban(context);
            User.CreditCardNumber  = StringHelper.GenerateCreditCardNumber(context);
            User.CreditCardExpiry = StringHelper.GenerateCreditCardExpiry(context);
            User.CreditCardCvv = StringHelper.GenerateCreditCardCvv(context);
            User.PhoneNumber = StringHelper.FixPhoneNumber(User.PhoneNumber);
            
            // Klasa HashPassword w /Helpers/PasswordHelper.cs szyfruje wpisane przez użytkownika hasło. 
            User.Password = PasswordHelper.HashPassword(User.Password);
            
            // User.PasswordConfirm jest pustym stringiem, żeby nie duplikować hasła w bazie danych.
            User.PasswordConfirm = string.Empty;

            // Wysyła dane wprowadzone przez użytkownika i zapisuje je w bazie danych.
            context.Users.Add(User);
            context.SaveChanges();
            
            // Przekierowuje na stronę logowania.
            return RedirectToPage("Login", new { generatedLogin = User.Login });
        }
    }
}