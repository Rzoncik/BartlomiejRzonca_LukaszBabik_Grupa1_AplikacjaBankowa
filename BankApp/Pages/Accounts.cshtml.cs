using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankApp.Models;
using BankApp.Helpers;

namespace BankApp.Pages
{
    public class AccountsModel(AppDbContext db) : BaseUserPageModel(db)
    {
        public List<DbUsers> Accounts { get; private set; } = [];
        public string? SelectedIban { get; private set; }
        
        public void OnGet() => LoadAccounts();

        // Tworzy nowy rachunek bankowy.
        public IActionResult OnPostCreate()
        {
            RedirectIfNotLoggedIn();

            var login    = User.Identity!.Name!;
            var template = context.Users.AsNoTracking().First(u => u.Login == login);

            var clone = new DbUsers
            {
                Login              = login,
                Password           = template.Password,
                Balance            = 0m,
                FirstName          = template.FirstName,
                LastName           = template.LastName,
                Nationality        = template.Nationality,
                Pesel              = template.Pesel,
                IdCard             = template.IdCard,
                City               = template.City,
                StreetAndBuilding  = template.StreetAndBuilding,
                Apartment          = template.Apartment,
                PostalCode         = template.PostalCode,
                PhoneNumber        = template.PhoneNumber,
                Email              = template.Email,
                Iban               = StringHelper.GenerateIban(context),
                CreditCardNumber   = StringHelper.GenerateCreditCardNumber(context),
                CreditCardExpiry   = StringHelper.GenerateCreditCardExpiry(context),
                CreditCardCvv      = StringHelper.GenerateCreditCardCvv(context),
                PasswordConfirm    = string.Empty
            };

            context.Users.Add(clone);
            context.SaveChanges();

            SetActiveAccount(clone);
            return RedirectToPage();
        }

        // Przełącza na inny rachunek.
        public IActionResult OnPostSwitch(string iban)
        {
            RedirectIfNotLoggedIn();

            var login   = User.Identity!.Name!;
            var account = context.Users.FirstOrDefault(u => u.Login == login && u.Iban == iban);
            if (account is null) return NotFound();

            SetActiveAccount(account);
            HttpContext.Session.SetString("Balance", account.Balance.ToString(CultureInfo.InvariantCulture));
            return RedirectToPage();
        }

        // Usuwa rachunek, jeśli nie jest aktualnie używany oraz jego środki są równe 0.
        public IActionResult OnPostDelete(string iban)
        {
            RedirectIfNotLoggedIn();

            var login   = User.Identity!.Name!;
            var account = context.Users.FirstOrDefault(u => u.Login == login && u.Iban == iban);
            if (account is null)                             return NotFound();
            
            if (Request.Cookies["SelectedIban"] == account.Iban)
                return Forbid();
            
            if (account.Balance != 0m)
            {
                ModelState.AddModelError(string.Empty, "Konto można usunąć tylko, gdy saldo wynosi 0,00 zł.");
                LoadAccounts();
                return Page();
            }
            
            if (context.Users.Count(u => u.Login == login) <= 1)
            {
                ModelState.AddModelError(string.Empty, "Nie można usunąć jedynego konta użytkownika.");
                LoadAccounts();
                return Page();
            }
            
            var hasTransfers = context.Transfers.Any(t => t.SenderUserId == account.UserId ||
                                                          t.ReceiverUserId == account.UserId);
            if (hasTransfers)
            {
                ModelState.AddModelError(string.Empty, "Nie można usunąć konta, dla którego istnieją przelewy historyczne.");
                LoadAccounts();
                return Page();
            }

            // Usuwa rachunek z bazy danych.
            using var tx = context.Database.BeginTransaction();
            try
            {
                context.Users.Remove(account);
                context.SaveChanges();
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }

            return RedirectToPage();
        }
        
        private void SetActiveAccount(DbUsers account)
        {
            Response.Cookies.Append("SelectedIban", account.Iban!, new CookieOptions
            {
                Expires   = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly  = true,
                Secure    = true,
                SameSite  = SameSiteMode.Strict
            });
            
            HttpContext.Session.SetInt32("UserId", account.UserId);
            HttpContext.Session.SetString("Balance", account.Balance.ToString(CultureInfo.InvariantCulture));
        }

        private void LoadAccounts()
        {
            var login = User.Identity!.Name!;

            Accounts = context.Users.Where(u => u.Login == login).OrderBy(u => u.UserId).ToList();
            if (Accounts.Count == 0) return;

            SelectedIban = Request.Cookies["SelectedIban"];

            if (SelectedIban is null || Accounts.All(a => a.Iban != SelectedIban))
                SelectedIban = Accounts.First().Iban;

            var active = Accounts.First(a => a.Iban == SelectedIban);
            
            if (Request.Cookies["SelectedIban"] != SelectedIban)
            {
                Response.Cookies.Append("SelectedIban", SelectedIban!, new CookieOptions
                {
                    Expires   = DateTimeOffset.UtcNow.AddDays(30),
                    HttpOnly  = true,
                    Secure    = true,
                    SameSite  = SameSiteMode.Strict
                });
            }

            if (CurrentUserId != active.UserId)
                HttpContext.Session.SetInt32("UserId", active.UserId);
        }
    }
}