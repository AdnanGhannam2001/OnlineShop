namespace OnlineShop.Data.Constants;

public static class DbConstants
{
    public const int NanoIdSize = 25;
    public const string ConnectionStringName = "SqlServer";
    public static readonly string ScriptsPath = Path.Combine("Sql", "Init");
    public static readonly string SeedScriptsPath = Path.Combine(ScriptsPath, "Seed");
}
