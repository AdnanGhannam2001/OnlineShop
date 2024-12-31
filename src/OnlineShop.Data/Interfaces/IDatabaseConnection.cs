using Microsoft.Data.SqlClient;

namespace OnlineShop.Data.Interfaces;

public interface IDatabaseConnection : IDisposable
{
    SqlConnection Connection { get; set; }
}