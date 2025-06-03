using BankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Pages
{
    public class DashboardModel(AppDbContext db) : BaseUserPageModel(db)
    {
        public string? FirstName { get; set; }
        
        public string? Iban { get; set; }
    
        public void OnGet()
        {
            FirstName = HttpContext.Session.GetString("FirstName");
            Iban = Request.Cookies["SelectedIban"];
        }
    }
}