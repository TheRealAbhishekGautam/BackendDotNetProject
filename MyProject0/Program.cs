// In the older versions (.Net 6 and below), there were two files program.cs and startup.cs but now
// both of these files are combined into a single file i.e. Progran.cs
// In this file we will have some service regestrations, pipelines and middlewares.
// Always remember whenever we have to configure a pipeline Program.cs is the file.
// Program.cs is the file which will be executed first inside any project since by-default routing is present here.
// All the dependency injections will be handelled here only.

using Microsoft.EntityFrameworkCore;
using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.Repository;
using MyProject0.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MyProject0.Utility;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmailSender, EmailSender>();

// We are basically registring a service AddDbContext (telling the project that we are using ef core) and inside it we are passing
// the option UseSqlServer and inside it we are passing the connection string that will be used for our connection.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

// By default the user that will be used to add the values to the database is IdentityUser and it's default Identity Role will be used
// builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();
// Now we are customizing the default Role.
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// These routes will always be defined after regestring the Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// Since all of the identity are in razor pages, we have to enable it to our project by regesting the razor pages into the pipeline
builder.Services.AddRazorPages();

// We are saying that all the variables inside the "Stripe" section of appsettings file will be mapped with the variables of the class StripeSettings
// Make sure the name of the variables are exactly the same in both the places.
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Pipeline basically means that if a request comes to your application how the application will handle that.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection() and app.UseStaticFiles() is used to access all of the static files defined inside the wwwroot file.
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication will always be added before Authrization
app.UseAuthentication();

app.UseAuthorization();
app.MapRazorPages();
// Telling that on the startup, Home controller will be called
app.MapControllerRoute(
    // Routing basically means that whenever the request come to the project where that request will be handelled.
    // Here if no controller is defined, Home controller will be called and if no action is defined then Index action will be called by-default.
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

// We have to pass that secret key to configure the Stripe inside our project.
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

// Basically for runnning the project.
app.Run();

