using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Configure secure cookie policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = CookieSecurePolicy.Always;
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
});

// 🔐 Configure Authentication with secure cookie settings
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;

        options.Cookie.Name = "MyApp.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// 🧱 MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🌐 Global error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Account/Error");
    app.UseHsts(); // 🔒 Strict Transport Security
}

// ✅ HTTPS redirection and static files
app.UseHttpsRedirection();
app.UseStaticFiles();

// ✅ Secure cookies
app.UseCookiePolicy();

// 🔐 Add authentication/authorization
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// 🔁 Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
