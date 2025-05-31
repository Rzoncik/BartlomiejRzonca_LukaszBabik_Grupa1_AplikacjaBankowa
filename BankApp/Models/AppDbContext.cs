//Tutaj deklarowana jest baza danyc, żeby ją wykorzystać w wybranym elemencie programu.

using Microsoft.EntityFrameworkCore;

namespace BankApp.Models
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<DbUsers> Users { get; set; }
        
        public DbSet<DbTransfers> Transfers { get; set; }
    }
}