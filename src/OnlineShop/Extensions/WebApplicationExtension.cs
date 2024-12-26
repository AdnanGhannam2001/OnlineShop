using DbUp;
using Serilog;
using static OnlineShop.Constants.DbConstants;

namespace OnlineShop.Extensions;

internal static class WebApplicationExtension
{
    public static bool HandleArgs(this WebApplication app, string[] args)
    {
        if (args.Any(x => x == "-c" || x == "--create"))
        {
            Log.Information("Initializing Database");
            
            if (!HandleInitDatabse(
                app.Environment.ContentRootPath,
                app.Configuration.GetConnectionString(ConnectionStringName)))
            {
                return false;
            }
        }

        return false;
    }

    private static bool HandleInitDatabse(string rootPath, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Log.Fatal("ConnectionString is required in 'appsettings.json'");
            return false;
        }

        DropDatabase.For.SqlDatabase(connectionString);
        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To.SqlDatabase(connectionString)
            .WithScriptsFromFileSystem(Path.Combine(rootPath, ScriptsPath))
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
        {
            Log.Fatal("Failed to Init Database:\n\t{Error}\n\t{Script}",
                result.Error, result.ErrorScript.Contents);
            return false;
        }

        return true;
    }
}
