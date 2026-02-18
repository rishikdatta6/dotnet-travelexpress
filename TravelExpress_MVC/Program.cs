using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelExpress.Models;
using TravelExpress.Services;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Fly.io PORT binding
// --------------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// --------------------
// MVC
// --------------------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// --------------------
// DATABASE (Fly Postgres)
// --------------------
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(databaseUrl))
{
    throw new Exception("DATABASE_URL is not set");
}

// ✅ IMPORTANT: pass DATABASE_URL directly
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(databaseUrl, o =>
    {
        o.EnableRetryOnFailure();
    });
});

// --------------------
// Identity
// --------------------
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

// --------------------
// Services
// --------------------
builder.Services.AddHttpClient<HotelApiService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

// --------------------
// Seed Admin + Roles (SAFE)
// --------------------
using (var scope = app.Services.CreateScope())
{
    await SeedData.Initialize(scope.ServiceProvider);
}

// --------------------
// Middleware
// --------------------
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
