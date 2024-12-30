using Microsoft.Data.SqlClient;

namespace OnlineShop.Data.Interfaces;

public interface IDatabaseConnection
{
    SqlConnection Connection { get; set; }
}