using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BankApp.Models;
using BankApp.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Pages
{
    public class CreditCardInfoModel(AppDbContext db) : BaseUserPageModel(db)
    {
        [BindProperty]
        [Required(ErrorMessage = "Wprowadź hasło.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        public bool ShowCardInfo { get; set; }

        public string CreditCardNumber { get; private set; } = string.Empty;
        
        public string CreditCardExpiry { get; private set; } = string.Empty;
        
        public string CreditCardCvv { get; private set; } = string.Empty;

        public void OnGet() => ShowCardInfo = false;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            
            RedirectIfNotLoggedIn();

            var user = await GetCurrentUserAsync();

            var hashedInput = PasswordHelper.HashPassword(Password);
            if (user.Password != hashedInput)
            {
                ModelState.AddModelError(nameof(Password), "Niepoprawne hasło.");
                return Page();
            }

            // Wyświetl informacje o karcie.
            CreditCardNumber = user.CreditCardNumber!;
            CreditCardExpiry = user.CreditCardExpiry!;
            CreditCardCvv = user.CreditCardCvv!;
            ShowCardInfo = true;

            // Usuń informacje o haśle użytkownika.
            Password = string.Empty;

            return Page();
        }
    }
}