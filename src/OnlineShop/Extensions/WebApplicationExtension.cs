using OnlineShop.Data;
using Serilog;
using static OnlineShop.Data.Constants.DbConstants;

namespace OnlineShop.Extensions;

internal static class WebApplicationExtension
{
    public static bool? HandleArgs(this WebApplication app, string[] args)
    {
        bool? exit = null;

        if (args.Any(x => x == "-c" || x == "--create"))
        {
            Log.Information("Initializing Database");
            exit = true;
            
            if (!DatabaseHelpers.Init(
                app.Environment.ContentRootPath,
                app.Configuration.GetConnectionString(ConnectionStringName)))
            {
                return false;
            }
        }

        if (args.Any(x => x == "-s" || x == "--seed"))
        {
            Log.Information("Seeding Database");
            exit = true;
            
            if (!DatabaseHelpers.Seed(
                app.Environment.ContentRootPath,
                app.Configuration.GetConnectionString(ConnectionStringName)))
            {
                return false;
            }
        }

        return exit;
    }
}
