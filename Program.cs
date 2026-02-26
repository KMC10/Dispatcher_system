using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DHLManagementSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Use connection string from appsettings.json
var connectionString = builder.Configuration
    .GetConnectionString("ApplicationDbContextConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


// 🔹 SEED ROLES (Dispatcher & Driver)
// 🔹 SEED ROLES + ASSIGN DISPATCHER
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Dispatcher", "Driver" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Assign Dispatcher role to your email
    string dispatcherEmail = "khulanikmc@gmail.com";
    var user = await userManager.FindByEmailAsync(dispatcherEmail);

    if (user != null && !await userManager.IsInRoleAsync(user, "Dispatcher"))
    {
        await userManager.AddToRoleAsync(user, "Dispatcher");
    }
}

app.Run();