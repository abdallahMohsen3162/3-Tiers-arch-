using Microsoft.AspNetCore.Authentication.Cookies;
using DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using businessLogic.Services.Interfaces;
using businessLogic.Services;
using businessLogic.Services.Services;
using DataLayer.Entities;
using businessLogic.Mappers;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();


//builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();


string baseUrl = "/Accounts";
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"{baseUrl}/Login";
    options.LogoutPath = $"{baseUrl}Logout";

});




var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    await ApplicationDbContext.SeedAsync(userManager);
}

if (!app.Environment.IsDevelopment())
{
    
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}





app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
