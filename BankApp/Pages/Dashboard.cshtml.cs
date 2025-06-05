using BankApp.Models;

namespace BankApp.Pages
{
    public class DashboardModel(AppDbContext db) : BaseUserPageModel(db)
    {
        public string? FirstName { get; set; }
        
        public string? Balance { get; set; }
        
        public string? Iban { get; set; }
    
        public void OnGet()
        {
            FirstName = HttpContext.Session.GetString("FirstName");
            Balance = HttpContext.Session.GetString("Balance");
            Iban = Request.Cookies["SelectedIban"];
        }
    }
}