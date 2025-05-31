using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BankApp.Models;
using BankApp.Helpers;

namespace BankApp.Pages
{
    public class AccountsModel(AppDbContext db) : BaseUserPageModel(db)
    {
        public List<DbUsers> Accounts { get; private set; } = [];
        
        public string? SelectedIban { get; private set; }
        
        // private int ActiveUserId { get; set; }

        /* ---------- GET ---------- */
        public void OnGet() => LoadAccounts();

        /* ---------- POST — create a brand‑new account ---------- */
        public IActionResult OnPostCreate()
        {
            RedirectIfNotLoggedIn();

            var login = User.Identity!.Name!;
            var template = context.Users.AsNoTracking().First(u => u.Login == login);

            var clone = new DbUsers
            {
                Login = login,
                Password = template.Password,
                Balance = 0m,
                FirstName = template.FirstName,
                LastName = template.LastName,
                Nationality = template.Nationality,
                Pesel = template.Pesel,
                IdCard = template.IdCard,
                City = template.City,
                StreetAndBuilding = template.StreetAndBuilding,
                Apartment = template.Apartment,
                PostalCode = template.PostalCode,
                PhoneNumber = template.PhoneNumber,
                Email = template.Email,
                Iban = StringHelper.GenerateIban(context),
                CreditCardNumber = StringHelper.GenerateCreditCardNumber(context),
                CreditCardExpiry = StringHelper.GenerateCreditCardExpiry(context),
                CreditCardCvv = StringHelper.GenerateCreditCardCvv(context),
                PasswordConfirm = string.Empty
            };

            context.Users.Add(clone);
            context.SaveChanges();

            SetActiveAccount(clone);         // cookie + session synced
            return RedirectToPage();
        }

        /* ---------- POST — switch to an existing account ---------- */
        public IActionResult OnPostSwitch(string iban)
        {
            var login   = User.Identity!.Name!;
            var account = context.Users.FirstOrDefault(u => u.Login == login && u.Iban == iban);
            if (account is null) return NotFound();

            SetActiveAccount(account);
            return RedirectToPage();
        }

        /* ---------- helpers ---------- */
        private void SetActiveAccount(DbUsers account)
        {
            // 1️⃣ persistent cookie for the UI
            Response.Cookies.Append("SelectedIban", account.Iban!, new CookieOptions
            {
                Expires   = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly  = true,
                Secure    = true,
                SameSite  = SameSiteMode.Strict
            });

            // 2️⃣ server‑side session for business logic (SendTransfer, etc.)
            HttpContext.Session.SetInt32("UserId", account.UserId);
        }

        private void LoadAccounts()
        {
            var login = User.Identity!.Name!;
            
            Accounts  = context.Users.Where(u => u.Login == login).OrderBy(u => u.UserId).ToList();
            
            if (Accounts.Count == 0) return;

            SelectedIban = Request.Cookies["SelectedIban"];
            
            if (SelectedIban is null || Accounts.All(a => a.Iban != SelectedIban))
                SelectedIban = Accounts.First().Iban;          // fallback

            var active = Accounts.First(a => a.Iban == SelectedIban);
            
            // ActiveUserId = active.UserId;

            // keep cookie & session in sync
            if (Request.Cookies["SelectedIban"] != SelectedIban)
            {
                Response.Cookies.Append("SelectedIban", SelectedIban!, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }

            if (CurrentUserId != active.UserId)
                HttpContext.Session.SetInt32("UserId", active.UserId);
        }
    }
}