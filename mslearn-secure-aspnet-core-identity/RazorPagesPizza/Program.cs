using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RazorPagesPizza.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using RazorPagesPizza.Services;
using QRCoder;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("RazorPagesPizzaAuthConnection")
    ?? throw new InvalidOperationException("Connection string 'RazorPagesPizzaAuthConnection' not found.");

builder.Services.AddDbContext<RazorPagesPizzaAuth>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<RazorPagesPizzaUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<RazorPagesPizzaAuth>();

builder.Services.AddRazorPages(options =>
    options.Conventions.AuthorizePage("/AdminsOnly", "Admin"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddSingleton(new QRCodeService(new QRCodeGenerator()));
builder.Services.AddAuthorization(options =>
    options.AddPolicy("Admin", policy =>
        policy.RequireAuthenticatedUser()
            .RequireClaim("IsAdmin", bool.TrueString)));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
