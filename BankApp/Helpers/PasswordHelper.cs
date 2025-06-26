// Paradygmat klas - klasa jest wykorzystywana w różnych plikach kodu.

using System.Security.Cryptography;
using System.Text;

namespace BankApp.Helpers
{
    public static class PasswordHelper
    {
        // Tutaj hasło jest szyfrowane za pomocą SHA256.
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}