// Paradygmat polimorfizmu

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Pages
{
    public class SendTransferModel(AppDbContext db) : BaseUserPageModel(db)
    {
        [BindProperty]
        [Required(ErrorMessage = "Pole rachunku odbiorcy jest wymagane.")]
        [RegularExpression(@"^(?:[A-Z]{2})?\d{26}$", ErrorMessage = "Podaj poprawny IBAN.")]
        public string ReceiverIban { get; set; } = null!;

        [BindProperty]
        [Required(ErrorMessage = "Kwota jest wymagana.")]
        [Range(0.01, 1_000_000_000, ErrorMessage = "Kwota jest wymagana.")]
        [RegularExpression(@"^\d+([.,]\d{1,2})?$", ErrorMessage = "Kwota może mieć maksymalnie dwie cyfry po przecinku.")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Wprowadź dane odbiorcy.")]
        public string ReceiversName { get; set; } = null!;
        
        [BindProperty]
        public string? Title { get; set; }
        
        public void OnGet() { }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            RedirectIfNotLoggedIn();

            var sender = await GetCurrentUserAsync();
            
            ReceiverIban = ReceiverIban?.Trim().Replace(" ", "").ToUpperInvariant() ?? "";
            if (!ReceiverIban.StartsWith("PL", StringComparison.OrdinalIgnoreCase))
                ReceiverIban = "PL" + ReceiverIban;

            var strategy = new InternalTransferProcessor(context, sender, ReceiverIban, Amount, ReceiversName, Title);
            var validationError = await strategy.ExecuteAsync();

            if (validationError is not null)
            {
                ModelState.AddModelError(validationError.MemberNames.First(), validationError.ErrorMessage!);
                return Page();
            }
            
            HttpContext.Session.SetString("Balance",
                sender.Balance.ToString(CultureInfo.InvariantCulture));
            return RedirectToPage("/Dashboard");
        }

        // Paradygmat polimorfizmu. Pozwala na rozbudowę o kolejne typy przelewów (SEPA, SWIFT).
        private abstract class TransferProcessor(
            AppDbContext db,
            DbUsers sender,
            string receiverIban,
            decimal amount,
            string receiversName,
            string? title)
        {
            protected readonly AppDbContext Db = db;
            protected readonly DbUsers Sender = sender;
            protected readonly string ReceiverIban = receiverIban;
            protected readonly decimal Amount = amount;
            protected readonly string ReceiversName = receiversName;
            protected readonly string? Title = title;
            
            public async Task<ValidationResult?> ExecuteAsync()
            {
                var error = await ValidateAsync();
                if (error is not null) return error;

                await ApplyAsync();
                return null;
            }
            protected abstract Task<ValidationResult?> ValidateAsync();
            protected abstract Task ApplyAsync();
        }
        
        private sealed class InternalTransferProcessor(
            AppDbContext db,
            DbUsers sender,
            string receiverIban,
            decimal amount,
            string receiversName,
            string? title) : TransferProcessor(db, sender, receiverIban, amount, receiversName, title)
        {
            private DbUsers? _receiver;

            protected override async Task<ValidationResult?> ValidateAsync()
            {
                _receiver = await Db.Users.SingleOrDefaultAsync(u => u.Iban == ReceiverIban);
                if (_receiver is null)
                    return new ValidationResult("W naszym banku nie ma konta o takim IBAN.", [nameof(SendTransferModel.ReceiverIban)]
                    );


                if (_receiver.UserId == Sender.UserId)
                    return new ValidationResult("Nie możesz wysłać przelewu na własne konto.", [nameof(SendTransferModel.ReceiverIban)]);
                    

                if (Sender.Balance < Amount)
                    return new ValidationResult("Niewystarczające środki.", [nameof(SendTransferModel.Amount)]);

                return null;
            }

            protected override async Task ApplyAsync()
            {
                await using var tx = await Db.Database.BeginTransactionAsync();
                try
                {
                    Sender.Balance  -= Amount;
                    _receiver!.Balance += Amount;

                    Db.Transfers.Add(new DbTransfers
                    {
                        SenderUserId = Sender.UserId,
                        ReceiverUserId = _receiver.UserId,
                        SenderIban = Sender.Iban!,
                        ReceiverIban = _receiver.Iban!,
                        Amount = Amount,
                        ReceiversName = ReceiversName,
                        Title = Title,
                        ExecutedAtUtc = DateTime.UtcNow,
                    });

                    await Db.SaveChangesAsync();
                    await tx.CommitAsync();
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            }
        }
    }
}