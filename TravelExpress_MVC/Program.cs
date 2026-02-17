using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelExpress.Models;
using TravelExpress.Services;

var builder = WebApplication.CreateBuilder(args);

// Fly port
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// --------------------
// DATABASE + IDENTITY
// --------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = false)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>();
}
else
{
    // ✅ Identity WITHOUT database (prevents DI crash)
    builder.Services.AddDefaultIdentity<ApplicationUser>()
        .AddRoles<IdentityRole>()
        .AddDefaultTokenProviders();
}

// HTTP Client
builder.Services.AddHttpClient<HotelApiService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

// --------------------
// SEED ONLY IF DB EXISTS
// --------------------
if (app.Environment.IsDevelopment() && !string.IsNullOrEmpty(connectionString))
{
    using var scope = app.Services.CreateScope();
    await SeedData.Initialize(scope.ServiceProvider);
}

// Middleware
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
