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
    .AddRoles<IdentityRole>() 
    .AddEntityFrameworkStores<ApplicationDbContext>();


//builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IRolesService, RolesService>();


builder.Services.AddAuthorization(options =>
{

    options.AddPolicy(AuthenticationConstants.Identity.Manage, policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == AuthenticationConstants.Identity.Create) ||
            context.User.HasClaim(c => c.Type == AuthenticationConstants.Identity.Delete) ||
            context.User.HasClaim(c => c.Type == AuthenticationConstants.Identity.Edit)
        ));


    options.AddPolicy(AuthenticationConstants.Identity.Create, policy =>
    policy.RequireClaim(AuthenticationConstants.Identity.Create));

    options.AddPolicy(AuthenticationConstants.Identity.Edit, policy =>
    policy.RequireClaim(AuthenticationConstants.Identity.Edit));

    options.AddPolicy(AuthenticationConstants.Identity.Delete, policy =>
    policy.RequireClaim(AuthenticationConstants.Identity.Delete));

    options.AddPolicy(AuthenticationConstants.Claims.Manage, policy =>
        policy.RequireClaim(AuthenticationConstants.Claims.Manage));


});


string baseUrl = "/Accounts";
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"{baseUrl}/Login";
    options.LogoutPath = $"{baseUrl}Logout";
    options.AccessDeniedPath = "/Errors/AccessDenied"; 

});




var app = builder.Build();

await DataLayer.Data.AutoGenerateUser.SeedAsync(app.Services);




if (!app.Environment.IsDevelopment())
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

app.Run();
