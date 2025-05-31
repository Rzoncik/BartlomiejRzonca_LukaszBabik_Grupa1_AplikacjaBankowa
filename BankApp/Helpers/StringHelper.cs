using System.Globalization;
using System.Text;
using BankApp.Models;

namespace BankApp.Helpers
{
    public static class StringHelper
    {
        // Klasa odpowiada za kapitalizację trzech pierwszych liter.
        public static string CapitalizeFirstThree(string input)
        {
            string firstThree = input.Substring(0, 3).ToUpper();
            string rest = input.Substring(3);
            return firstThree + rest;
        }
        
        // Klasa odpowiada za kapitalizację pierwszej litery wpisu, do którego się ją zaimplementuje.
        public static string CapitalizeFirstChar(string input)
        {
            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
        
        // Generuje 28-znakowy rachunek IBAN.
        public static string GenerateIban(AppDbContext context)
        {
            string iban;
            do
            {
                var sb = new StringBuilder(26);
                for (int i = 0; i < 26; i++)
                    sb.Append(RandomizeHelper.Random.Next(0, 10));
                iban = $"PL{sb}";
            }
            while (context.Users.Any(u => u.Iban == iban));

            return iban;
        }
        
        // Generuje 16-cyfrowy ciąg liczb.
        public static string GenerateCreditCardNumber(AppDbContext context)
        {
            string creditCardNumber;
            do
            {
                var sb = new StringBuilder(16);
                
                for (int i = 0; i < 16; i++)
                    sb.Append(RandomizeHelper.Random.Next(0, 10));

                creditCardNumber = sb.ToString();
            }
            while (context.Users.Any(u => u.CreditCardNumber == creditCardNumber));

            return creditCardNumber;
        }
        
        public static string GenerateCreditCardExpiry(AppDbContext context)
        {
            DateTime expiry = DateTime.UtcNow.AddMonths(60);
            
            return expiry.ToString("MM'/'yy", CultureInfo.InvariantCulture);
        }
        
        public static string GenerateCreditCardCvv(AppDbContext context)
        {
            return RandomizeHelper.Random.Next(100, 1000).ToString("D3");
        }
        
        public static string FixPhoneNumber(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return string.Empty;
            
            if (raw.StartsWith('+'))
                return raw;
            
            return "+48" + raw;
        }
    }
}