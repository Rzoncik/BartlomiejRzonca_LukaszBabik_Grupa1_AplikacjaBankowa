using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BankApp.Models;
using BankApp.Helpers;

namespace BankApp.Pages
{
    // Keeps each user logically split into "accounts" that share the same credentials
    public class AccountsModel(AppDbContext db) : BaseUserPageModel(db)
    {
        public List<DbUsers> Accounts { get; private set; } = [];
        public string? SelectedIban { get; private set; }

        /* ---------- GET ---------- */
        public void OnGet() => LoadAccounts();

        /* ---------- POST — create a brand‑new account ---------- */
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

            SetActiveAccount(clone);          // cookie + session synced
            return RedirectToPage();          // refresh
        }

        /* ---------- POST — switch to an existing account ---------- */
        public IActionResult OnPostSwitch(string iban)
        {
            RedirectIfNotLoggedIn();

            var login   = User.Identity!.Name!;
            var account = context.Users.FirstOrDefault(u => u.Login == login && u.Iban == iban);
            if (account is null) return NotFound();

            SetActiveAccount(account);
            HttpContext.Session.SetString("Balance", account.Balance.ToString(CultureInfo.InvariantCulture));
            return RedirectToPage();          // reload -> OnGet()
        }

        /* ---------- POST — delete an INACTIVE account (saldo = 0) ---------- */
        public IActionResult OnPostDelete(string iban)
        {
            RedirectIfNotLoggedIn();

            var login   = User.Identity!.Name!;
            var account = context.Users.FirstOrDefault(u => u.Login == login && u.Iban == iban);
            if (account is null)                             return NotFound();

            // 🔐 forbid removing the active account
            if (Request.Cookies["SelectedIban"] == account.Iban)
                return Forbid();

            // 🔐 saldo musi być dokładnie 0,00 zł
            if (account.Balance != 0m)
            {
                ModelState.AddModelError(string.Empty, "Konto można usunąć tylko, gdy saldo wynosi 0,00 zł.");
                LoadAccounts();
                return Page();
            }

            // 🔐 nie można usunąć ostatniego konta użytkownika
            if (context.Users.Count(u => u.Login == login) <= 1)
            {
                ModelState.AddModelError(string.Empty, "Nie można usunąć jedynego konta użytkownika.");
                LoadAccounts();
                return Page();
            }

            // 🔐 musi być brak przelewów powiązanych (nadawcą lub odbiorcą)
            var hasTransfers = context.Transfers.Any(t => t.SenderUserId == account.UserId ||
                                                          t.ReceiverUserId == account.UserId);
            if (hasTransfers)
            {
                ModelState.AddModelError(string.Empty, "Nie można usunąć konta, dla którego istnieją przelewy historyczne.");
                LoadAccounts();
                return Page();
            }

            /* —— delete inside a DB transaction —— */
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

            return RedirectToPage();          // back to refreshed list
        }

        /* ---------- helpers ---------- */
        private void SetActiveAccount(DbUsers account)
        {
            // 1️⃣ persistent cookie for UI
            Response.Cookies.Append("SelectedIban", account.Iban!, new CookieOptions
            {
                Expires   = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly  = true,
                Secure    = true,
                SameSite  = SameSiteMode.Strict
            });

            // 2️⃣ server‑side session for business logic
            HttpContext.Session.SetInt32("UserId", account.UserId);
        }

        private void LoadAccounts()
        {
            var login = User.Identity!.Name!;

            Accounts = context.Users.Where(u => u.Login == login)
                                     .OrderBy(u => u.UserId)
                                     .ToList();
            if (Accounts.Count == 0) return;

            SelectedIban = Request.Cookies["SelectedIban"];

            if (SelectedIban is null || Accounts.All(a => a.Iban != SelectedIban))
                SelectedIban = Accounts.First().Iban;           // fallback

            var active = Accounts.First(a => a.Iban == SelectedIban);

            /* keep cookie & session in sync */
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