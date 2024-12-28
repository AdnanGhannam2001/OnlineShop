using Microsoft.Data.SqlClient;
using OnlineShop.Interfaces;

namespace OnlineShop.Data;

public sealed class DapperDatabaseConnection : IDatabaseConnection
{
    public DapperDatabaseConnection(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new NullReferenceException($"Connection string in '{nameof(DapperDatabaseConnection)}' can't be null");
        }

        Connection = new(connectionString);
    }

    public SqlConnection Connection { get; set; }
}