using Microsoft.AspNetCore.Identity;
using ProductManagement.Data;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data.Repositories.ProductCrud;
using ProductManagement.Data.Repositories.UserCrud;
using ProductManagement.Data.Repositories.Account;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});



builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Add UserManager and SignInManager to DI container
/*builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>();*/


// Configure the authorization policies

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminPolicy", policy =>
        policy.RequireRole("SuperAdmin"));
});

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISuperAdminService, SuperAdminService>();
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    SeedData.Initialize(serviceProvider).Wait();
}

// Configure the HTTP request pipeline.
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
    pattern: "{controller=Account}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Admin",
    pattern: "{controller=Admin}/{action=AdminDashboard}/{id?}");

app.MapControllerRoute(
    name: "SuperAdmin",
    pattern: "{controller=SuperAdmin}/{action=SuperAdminDashboard}/{id?}");
app.MapControllerRoute(
    name: "User",
    pattern: "{controller=User}/{action=USerDashboard}/{id?}");


app.Run();
