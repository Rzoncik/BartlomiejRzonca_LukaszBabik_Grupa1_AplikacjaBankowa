using BankApp.Models;

namespace BankApp.Pages
{
    public class DashboardModel(AppDbContext db) : BaseUserPageModel(db)
    {
        public string? FirstName { get; set; }
    
        public void OnGet()
        {
            FirstName = HttpContext.Session.GetString("FirstName");
        }
    }
}