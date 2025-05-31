// Tutaj wpisywane sa dane z bazy danych, na których będzie operować cały program.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApp.Models
{
    public class DbUsers
    {
        [Key]
        public int UserId { get; set; }
        
        [Display(Name = "Login")]
        [MaxLength(8)]
        public string? Login { get; set; }
        
        [Required(ErrorMessage = "Wprowadź hasło.")]
        [Display(Name = "Hasło")]
        [MaxLength(256)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", ErrorMessage = "Hasło musi mieć co najmniej 8 znaków, zawierać wielką literę, cyfrę i znak specjalny.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        
        [Required(ErrorMessage = "Powtórz hasło.")]
        [Display(Name = "Wprowadź hasło ponownie")]
        [MaxLength(256)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła nie są zgodne.")]
        public string? PasswordConfirm { get; set; }
        
        [Display(Name = "Balance")]
        [Range(0.01, 1_000_000_000)]
        [Column(TypeName="decimal(18,2)")]
        public decimal Balance { get; set; } = 1000;
        
        [Display(Name = "Iban")]
        [MaxLength(28)]
        [RegularExpression(@"^[A-Z]{2}\d{26}$")]
        public string? Iban { get; set; }
        
        [Display(Name = "CreditCardNumber")]
        [MaxLength(16)]
        public string? CreditCardNumber { get; set; }
        
        [Display(Name = "CreditCardExpiry")]
        [MaxLength(5)]
        public string? CreditCardExpiry { get; set; }
        
        [Display(Name = "CreditCardCvv")]
        [MaxLength(3)]
        public string? CreditCardCvv { get; set; }
        
        [Required(ErrorMessage = "Wprowadź imię")]
        [Display(Name = "Imię")]
        [MaxLength(64)]
        public string FirstName { get; set; } = null!;
        
        [Required(ErrorMessage = "Wprowadź nazwisko.")]
        [Display(Name = "Nazwisko")]
        [MaxLength(64)]
        public string LastName { get; set; } = null!;
        
        [Required(ErrorMessage = "Wprowadź obywatelstwo.")]
        [Display(Name = "Obywatelstwo")]
        [MaxLength(64)]
        public string Nationality { get; set; } = null!;
        
        [Required(ErrorMessage = "Wprowadź Pesel.")]
        [Display(Name = "PESEL")]
        [MaxLength(11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL musi składać się z 11 cyfr.")]
        public string Pesel { get; set; } = null!;
        
        [Required(ErrorMessage = "Wprowadź serię i numer dowodu osobistego.")]
        [Display(Name = "Seria i numer dowodu osobistego")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Dowód musi zawierać dokładnie 9 znaków.")]
        [RegularExpression(@"^[a-zA-Z]{3}[0-9]{6}$", ErrorMessage = "Dowód musi składać się z 3 liter i 6 cyfr (np. ABC123456).")]
        public string IdCard { get; set; } = null!;

        
        [Required(ErrorMessage = "Wprowadź miasto.")]
        [Display(Name = "Miasto")]
        [MaxLength(64)]
        public string City { get; set; } = null!;
        
        [Required(ErrorMessage = "Wprowadź ulicę i numer budynku.")]
        [Display(Name = "Ulica i numer budynku")]
        [MaxLength(64)]
        public string StreetAndBuilding { get; set; } = null!;
        
        [Display(Name = "Lokal")]
        [MaxLength(64)]
        public string? Apartment { get; set; }
        
        [Required(ErrorMessage = "Wprowadź kod pocztowy.")]
        [Display(Name = "Kod pocztowy")]
        [MaxLength(6)]
        [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Kod pocztowy musi mieć format 00-000.")]
        public string PostalCode { get; set; } = null!;
        
        [Required(ErrorMessage = "Wprowadź numer telefonu.")]
        [Display(Name = "Numer telefonu")]
        [MaxLength(12)]
        [RegularExpression(@"^\+?\d{9,11}$", ErrorMessage = "Wprowadź poprawny numer telefonu.")]
        public string PhoneNumber { get; set; } = null!;
        
        [Required(ErrorMessage = "Wprowadź email.")]
        [Display(Name = "Email")]
        [MaxLength(256)]
        [EmailAddress(ErrorMessage = "Wprowadź poprawny adres e-mail.")]
        public string Email { get; set; } = null!;
    }
}