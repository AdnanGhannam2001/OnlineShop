using OnlineShop.Constants;
using OnlineShop.Data;
using OnlineShop.Extensions;
using OnlineShop.Interfaces;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDatabaseConnection, DapperDatabaseConnection>(
    _ => new DapperDatabaseConnection(builder.Configuration.GetConnectionString(DbConstants.ConnectionStringName)));

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

return 0;
