using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using BankApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(opt =>
{
    // optional plain HTTP for old links / health checks
    opt.ListenAnyIP(5000);

    // HTTPS (uses the cert we just created)
    opt.ListenAnyIP(5001, listen =>
    {
        listen.Protocols = HttpProtocols.Http1AndHttp2;
        listen.UseHttps();           // auto‑picks ~/.aspnet/https/*.pfx
    });
});


builder.Services.AddRazorPages();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath        = "/Login";      // where unauth users are redirected
        // opt.AccessDeniedPath = "/Forbidden";  // optional
        opt.ExpireTimeSpan   = TimeSpan.FromMinutes(30);
        opt.SlidingExpiration = true;         // refresh on activity
    });

builder.Services.AddAuthorization();
builder.Services.AddSession();


//Połączenie z bazą danych.
var connectionString = "Data Source=Database.db";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();            // *before* auth if you want session during sign‑in
app.UseAuthentication();     // cookie validation happens here
app.UseAuthorization();

app.MapRazorPages();

app.Run();
