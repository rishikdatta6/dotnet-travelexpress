using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelExpress.Models;
using TravelExpress.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// DATABASE (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddHttpClient<HotelApiService>();
builder.Services.AddAuthorization();

var app = builder.Build();

// Seed (keep commented for now)
using (var scope = app.Services.CreateScope())
{
    await SeedData.Initialize(scope.ServiceProvider);
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();