using App.Data;
using App.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MVCApplication.Areas.Product.Service;
using MVCTest.ExtendMethods;
using MVCTest.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


//Thêm vào DbContext BlogContext vào d?ch v?
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AppContext");
    options.UseMySQL(connectionString);
});

//Kích ho?t s? d?ng Options
builder.Services.AddOptions();
//Ðãng k? d?ch v? cho MailSettings
var mailSettingsOption = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettingsOption);
builder.Services.AddSingleton<IEmailSender, SendMailService>();

//Ðãng k? d?ch v? cho Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

//Ðãng k? ?y quy?n truy c?p khi vào 1 trang
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Thi?t l?p v? Password
    options.Password.RequireDigit = false; // Không b?t ph?i có s?
    options.Password.RequireLowercase = false; // Không b?t ph?i có ch? thý?ng
    options.Password.RequireNonAlphanumeric = false; // Không b?t k? t? ð?c bi?t
    options.Password.RequireUppercase = false; // Không b?t bu?c ch? in
    options.Password.RequiredLength = 3; // S? k? t? t?i thi?u c?a password
    options.Password.RequiredUniqueChars = 1; // S? k? t? riêng bi?t

    // C?u h?nh Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Th?t b?i 5 l?n th? khóa
    options.Lockout.AllowedForNewUsers = true;

    // C?u h?nh v? User.
    options.User.AllowedUserNameCharacters = // các k? t? cho phép trong tên c?a user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nh?t và ch? ðý?c s? d?ng 1 l?n

    // C?u h?nh ðãng nh?p.
    options.SignIn.RequireConfirmedEmail = true;            // C?u h?nh xác th?c ð?a ch? email (email ph?i t?n t?i)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác th?c s? ði?n tho?i
    options.SignIn.RequireConfirmedAccount = true;     //Xác th?c tài kho?n sau khi ðãng k?
});

builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();
builder.Services.AddTransient<CartService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ViewManageMenu", builder =>
    {
        builder.RequireAuthenticatedUser();
        builder.RequireRole(RoleName.Administrator);
    });
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession((option) =>
{
    option.Cookie.Name = "buiquochuy";
    option.IdleTimeout = new TimeSpan(0, 30, 0);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(),"Uploads")
    ),
    RequestPath = "/contents"
});

app.UseSession();

app.AddStatusCodePage();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    areaName: "Database"
);
app.MapAreaControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    areaName: "Contact"
);

app.MapAreaControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    areaName: "Product"
);


app.MapAreaControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    areaName: "Blog"
);

//URL: /{controller}/{action}/{id?}
// Abc/Xyz => Controller: Abc, Action: Xyz
//Default: controller=Home, action = Index

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.Run();
