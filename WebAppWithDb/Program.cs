using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAppWithDb.Data;
using WebAppWithDb.Data.Infrastructure;
var builder = WebApplication.CreateBuilder(args);

//----------------------DB-----------------
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


//--------------------identity and roles-----------------------------

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; // to-do -- true
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
var app = builder.Build();

//--------------------role manager-------------------
using (var scope = app.Services.CreateScope()) //to-do what's scoped service
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    string[] roles = new[] { "Driver", "Student", "Admin" }; 
    foreach (var r in roles)
        if (!await roleManager.RoleExistsAsync(r))
            await roleManager.CreateAsync(new IdentityRole(r));

    var adminEmail = "hussein.admin@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if(adminUser == null) // going to run for the first time only, or could be more for another reasons
    {
        adminUser = new IdentityUser 
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            PhoneNumber = "12345678910"
        };
        await userManager.CreateAsync(adminUser, "Qwerty@@123");
        await userManager.AddToRoleAsync(adminUser, Roles.Admin);// or simply>> "Admin" as second param
    }

}

    // Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=AdminDashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
