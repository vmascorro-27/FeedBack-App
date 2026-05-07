using FeedBack_APP.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/LOGIN/Index";
        options.AccessDeniedPath = "/LOGIN/Index";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.Name = "FeedBackApp.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddScoped<FeedbackDbContext>(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnectionFeedBack")
        ?? throw new InvalidOperationException("The 'MySqlConnectionFeedBack' connection string was not found.");

    return new FeedbackDbContext(connectionString);
});

builder.Services.AddScoped<MasterDbContext>(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnectionMaster")
        ?? throw new InvalidOperationException("The 'MySqlConnectionMaster' connection string was not found.");

    return new MasterDbContext(connectionString);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/LOGIN/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LOGIN}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
