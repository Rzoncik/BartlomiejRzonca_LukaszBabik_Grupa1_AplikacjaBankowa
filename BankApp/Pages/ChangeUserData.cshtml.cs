using System.ComponentModel.DataAnnotations;
using BankApp.Helpers;
using BankApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Pages
{
    public class ChangeUserDataModel(AppDbContext db) : BaseUserPageModel(db)
    {
        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [BindProperty]
            [Display(Name = "Hasło")]
            [Required]
            [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", ErrorMessage = "Hasło musi mieć co najmniej 8 znaków, zawierać wielką literę, cyfrę i znak specjalny.")]
            public string Password { get; set; } = null!;

            [BindProperty]
            [Display(Name = "Wprowadź hasło ponownie")]
            [Required]
            [Compare(nameof(Password), ErrorMessage = "Hasła nie są zgodne.")]
            public string PasswordConfirm { get; set; } = null!;

            [BindProperty]
            [Display(Name = "Miasto")]
            [Required]
            public string City { get; set; } = null!;
            
            [BindProperty]
            [Display(Name = "Ulica i numer budynku")]
            [Required]
            public string StreetAndBuilding { get; set; } = null!;

            [BindProperty]
            [Display(Name = "Lokal")]
            public string? Apartment { get; set; }
            
            [BindProperty]
            [Display(Name = "Kod pocztowy")]
            [Required]
            [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Format 00-000")]
            public string PostalCode { get; set; } = null!;
            
            [BindProperty]
            [Display(Name = "Numer telefonu")]
            [Required]
            [RegularExpression(@"^\+?\d{9,11}$")]
            public string PhoneNumber  { get; set; } = null!;
            
            [BindProperty]
            [Display(Name = "Email")]
            [Required]
            [MaxLength(256)]
            [EmailAddress]
            public string Email { get; set; } = null!;
        }
        
        public IActionResult OnGet()
        {
            RedirectIfNotLoggedIn();

            var sample = context.Users.Find(CurrentUserId);
            // if (sample is null) return RedirectToPage("/Login");

            Input = new InputModel
            {
                City = sample!.City,
                StreetAndBuilding = sample.StreetAndBuilding,
                Apartment = sample.Apartment,
                PostalCode = sample.PostalCode,
                PhoneNumber = sample.PhoneNumber,
                Email = sample.Email
            };
            return Page();
        }

        /*———— UPDATE ALL RECORDS WITH SAME LOGIN ————*/
        public async Task<IActionResult> OnPostAsync()
        {
            RedirectIfNotLoggedIn();
            
            if (!ModelState.IsValid)
                return Page();

            var current = await context.Users.FindAsync(CurrentUserId);

            var login = current!.Login;

            await using var tx = await context.Database.BeginTransactionAsync();
            
            var affected = await context.Users.Where(u => u.Login == login).ToListAsync();

            foreach (var u in affected)
            {
                u.Password = PasswordHelper.HashPassword(Input.Password);
                u.City = StringHelper.CapitalizeFirstChar(Input.City);
                u.StreetAndBuilding = StringHelper.CapitalizeFirstChar(Input.StreetAndBuilding);
                u.Apartment = Input.Apartment;
                u.PostalCode = Input.PostalCode;
                u.PhoneNumber = StringHelper.FixPhoneNumber(Input.PhoneNumber);
                u.Email = Input.Email;
            }

            await context.SaveChangesAsync();
            await tx.CommitAsync();

            return RedirectToPage("/Dashboard");
        }
    }
}