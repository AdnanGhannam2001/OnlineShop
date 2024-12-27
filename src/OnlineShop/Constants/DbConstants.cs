namespace OnlineShop.Constants;

public static class DbConstants
{
    public const string ConnectionStringName = "SqlServer";
    public static readonly string ScriptsPath = Path.Combine("Data", "Sql", "Init");
    public static readonly string SeedScriptsPath = Path.Combine(ScriptsPath, "Seed");
}
