using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using OmnitakSupportHub;
using OmnitakSupportHub.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<OmnitakContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register your custom user service (in-memory or database-backed)
builder.Services.AddScoped<IAuthService, InMemoryUserService>();

// ✅ Add cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";           // Redirect to this on [Authorize]
        options.AccessDeniedPath = "/Account/AccessDenied"; // Optional
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // ✅ Required for serving CSS/JS

app.UseRouting();

app.UseAuthentication();  // ✅ Add authentication middleware
app.UseAuthorization();   // Already present

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"); // Default route to login

app.Run();
