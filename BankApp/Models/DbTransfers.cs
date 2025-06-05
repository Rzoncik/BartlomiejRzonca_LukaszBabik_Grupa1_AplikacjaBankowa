// Tutaj wpisywane sa dane z tabeli Transfers z bazy danych, na których będzie operować cały program.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApp.Models
{
    public class DbTransfers
    {
        [Key]
        public int TransferId { get; set; }
    
        [ForeignKey(nameof(Sender))]
        public int SenderUserId { get; set; }
        public DbUsers Sender { get; set; } = null!;

        [ForeignKey(nameof(Receiver))]
        public int ReceiverUserId { get; set; }
        public DbUsers Receiver { get; set; } = null!;
    
        [Required]
        [MaxLength(28)]
        public string SenderIban { get; set; } = null!;

        [Required(ErrorMessage = "Pole rachunku odbiorcy jest wymagane.")]
        [MaxLength(28)]
        [RegularExpression(@"^(?:[A-Z]{2})?\d{26}$", ErrorMessage = "Podaj poprawny IBAN.")]
        public string ReceiverIban { get; set; } = null!;
        
        [Required(ErrorMessage = "Kwota jest wymagana.")]
        [Range(typeof(decimal), "0.01", "1000000000", ErrorMessage = "Kwota musi mieścić się w zakresie od 0,01 do 1000000000.")]
        [RegularExpression(@"^\d+([.,]\d{1,2})?$", ErrorMessage = "Kwota może mieć maksymalnie dwie cyfry po przecinku.")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal Amount { get; set; }

        [MaxLength(256)]
        public string ReceiversName { get; set; } = null!;
    
        [MaxLength(256)]
        public string? Title { get; set; }

        public DateTime ExecutedAtUtc { get; set; } = DateTime.UtcNow;
    }
}