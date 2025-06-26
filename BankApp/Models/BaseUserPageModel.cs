// Paradygmat dziedziczenia - klasa BaseUserPageModel dziedziczy cechy klasy PageModel. Ta klasa jest wykorzystywana jedynie gdy użytkownik jest zalogowany np. /Dashboard lub SendTransfer.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Models
{
    [Authorize]
    public abstract class BaseUserPageModel(AppDbContext db) : PageModel
    {
        protected AppDbContext context => db;
        
        public int? CurrentUserId => HttpContext.Session.GetInt32("UserId");
        
        protected IActionResult? RedirectIfNotLoggedIn()
            => CurrentUserId is null ? RedirectToPage("/Login") : null;
        
        protected Task<DbUsers> GetCurrentUserAsync()
            => context.Users.SingleAsync(u => u.UserId == CurrentUserId);
        
        [TempData]
        public string? ToastMessage { get; set; }
    }
}