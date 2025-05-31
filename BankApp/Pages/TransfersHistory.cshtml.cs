using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankApp.Models;

namespace BankApp.Pages
{
    public class TransfersHistoryModel(AppDbContext db) : BaseUserPageModel(db)
    {
        public List<DbTransfers> Transfers { get; private set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            RedirectIfNotLoggedIn();

            Transfers = await context.Transfers
                .Include(t => t.Sender)
                .Include(t => t.Receiver)
                .Where(t => t.SenderUserId == CurrentUserId || t.ReceiverUserId == CurrentUserId)
                .OrderByDescending(t => t.ExecutedAtUtc)
                .ToListAsync();

            return Page();
        }
    }
}