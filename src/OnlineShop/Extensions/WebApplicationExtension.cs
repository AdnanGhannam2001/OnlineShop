using DbUp;
using Serilog;
using static OnlineShop.Constants.DbConstants;

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
            
            if (!HandleInitDatabse(
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
            
            if (!HandleSeedDatabase(
                app.Environment.ContentRootPath,
                app.Configuration.GetConnectionString(ConnectionStringName)))
            {
                return false;
            }
        }

        return exit;
    }

    // TODO: move to a seperate class
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

    // TODO: move to a seperate class
    private static bool HandleSeedDatabase(string rootPath, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Log.Fatal("ConnectionString is required in 'appsettings.json'");
            return false;
        }

        var full_path = Path.Combine(rootPath, SeedScriptsPath);
        if (!Directory.Exists(full_path))
        {
            Log.Error("No seeding scripts were found, please use 'generate-seed.py' script to generate the seed");
            return false;
        }

        var upgrader = DeployChanges.To.SqlDatabase(connectionString)
            .WithScriptsFromFileSystem(full_path)
            .Build();

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
        {
            Log.Fatal("Failed to Seed Database:\n\t{Error}\n\t{Script}",
                result.Error, result.ErrorScript.Contents);
            return false;
        }

        return true;
    }
}
