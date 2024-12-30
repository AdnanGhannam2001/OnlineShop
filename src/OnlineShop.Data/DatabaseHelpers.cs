using DbUp;
using Serilog;
using static OnlineShop.Data.Constants.DbConstants;

namespace OnlineShop.Data;

public static class DatabaseHelpers
{
    public static bool Init(string rootPath, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Log.Fatal("ConnectionString is required");
            return false;
        }

        DropDatabase.For.SqlDatabase(connectionString);
        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To.SqlDatabase(connectionString)
            .WithScriptsFromFileSystem(Path.Combine(rootPath + ".Data", ScriptsPath))
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

    public static bool Seed(string rootPath, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Log.Fatal("ConnectionString is required in 'appsettings.json'");
            return false;
        }

        var full_path = Path.Combine(rootPath + ".Data", SeedScriptsPath);
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