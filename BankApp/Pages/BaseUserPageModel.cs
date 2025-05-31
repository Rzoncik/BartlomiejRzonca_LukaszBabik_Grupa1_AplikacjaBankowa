// Paradygmat dziedziczenia.

using BankApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Pages
{
    [Authorize]
    public abstract class BaseUserPageModel(AppDbContext db) : PageModel
    {
        protected AppDbContext context => db;

        /// <summary>Current user id from session; <c>null</c> when not logged in.</summary>
        public int? CurrentUserId => HttpContext.Session.GetInt32("UserId");

        /// <summary>Redirect to “/Login” if nobody is logged in; otherwise <c>null</c>.</summary>
        protected IActionResult? RedirectIfNotLoggedIn()
            => CurrentUserId is null ? RedirectToPage("/Login") : null;

        /// <summary>Convenience helper that throws if the user is not found.</summary>
        protected Task<DbUsers> GetCurrentUserAsync()
            => context.Users.SingleAsync(u => u.UserId == CurrentUserId);
    }
}