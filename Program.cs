using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DHLManagementSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// =========================
// Database Configuration
// =========================
var connectionString = builder.Configuration
    .GetConnectionString("ApplicationDbContextConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// =========================
// Identity Configuration
// =========================
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
// .AddDefaultUI(); // Not needed for .NET 8 without Identity.UI

// =========================
// MVC + Razor Pages
// =========================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// =========================
// Middleware
// =========================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// =========================
// Routes
// =========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// =========================
// Seed roles & Dispatcher account
// =========================
await SeedRolesAndDispatcherAsync(app.Services);

app.Run();

// =========================
// Method: Seed Roles + Dispatcher
// =========================
static async Task SeedRolesAndDispatcherAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var sp = scope.ServiceProvider;

    var context = sp.GetRequiredService<ApplicationDbContext>();
    var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = sp.GetRequiredService<UserManager<IdentityUser>>();

    // Apply pending migrations
    await context.Database.MigrateAsync();

    // Define roles
    string[] roles = { "Dispatcher", "Driver" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Create Dispatcher account
    string dispatcherEmail = "dispatcher@example.com"; // change this to your email
    string dispatcherPassword = "StrongPassword123!";  // change this to your desired password

    var user = await userManager.FindByEmailAsync(dispatcherEmail);

    if (user == null)
    {
        user = new IdentityUser
        {
            UserName = dispatcherEmail,
            Email = dispatcherEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, dispatcherPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                Console.WriteLine($"Error creating user: {error.Description}");
        }
    }

    // Ensure the Dispatcher role is assigned
    if (!await userManager.IsInRoleAsync(user, "Dispatcher"))
    {
        await userManager.AddToRoleAsync(user, "Dispatcher");
        Console.WriteLine("Dispatcher account created and assigned role successfully!");
    }
}