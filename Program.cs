using CarpooliDotTN.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using CarpooliDotTN.Services;
using CarpooliDotTN.DAL.IRepositories;
using CarpooliDotTN.DAL.Repositories;
using CarpooliDotTN.DAL.IServices;
using CarpooliDotTN.DAL.Services;

var builder = WebApplication.CreateBuilder(args);
CarpooliDbContext.InitializeDatabase();
// Add services to the container.
builder.Services.AddDbContext<CarpooliDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CarpooliDbContextConnection")));
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDefaultIdentity<User>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<CarpooliDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddTransient<CarpooliDotTN.Services.IEmailSender, EmailSender>(); ;
builder.Services.AddScoped<IDemandRepository, DemandRepository>();
builder.Services.AddScoped<ICarpoolRepository, CarpoolRepository>();
builder.Services.AddScoped<ICarpoolService, CarpoolService>();
builder.Services.AddScoped<IDemandService, DemandService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // ... other non-development middleware
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
