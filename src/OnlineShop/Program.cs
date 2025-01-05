using Microsoft.AspNetCore.Authentication.Cookies;
using OnlineShop.Data.Constants;
using OnlineShop.Extensions;
using Serilog;
using OnlineShop.Data.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(config =>
    {
        config.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie();

builder.Services.AddData(builder.Configuration.GetConnectionString(DbConstants.ConnectionStringName));

var app = builder.Build();

var success = app.HandleArgs(args);
if (success == false)
{
    return -1;
}
else if (success == true)
{
    return 0;
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Errors/Index");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}");

app.Run();

return 0;
