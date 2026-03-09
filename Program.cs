using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DHLManagementSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Get connection string
var connectionString = builder.Configuration
    .GetConnectionString("ApplicationDbContextConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

// Configure database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Configure Identity (for .NET 8)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
//.AddDefaultUI();

// MVC + Razor
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Middleware
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Async role seeding
await SeedRolesAndAdminAsync(app.Services);

app.Run();


// =========================
// Async method for seeding roles/admin
// =========================
static async Task SeedRolesAndAdminAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var sp = scope.ServiceProvider;

    var context = sp.GetRequiredService<ApplicationDbContext>();
    var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = sp.GetRequiredService<UserManager<IdentityUser>>();

    // Apply migrations
    await context.Database.MigrateAsync();

    string[] roles = { "Dispatcher", "Driver" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    string dispatcherEmail = "khulanikmc@gmail.com";
    var user = await userManager.FindByEmailAsync(dispatcherEmail);

    if (user != null && !await userManager.IsInRoleAsync(user, "Dispatcher"))
        await userManager.AddToRoleAsync(user, "Dispatcher");
}