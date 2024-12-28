using Microsoft.Data.SqlClient;

namespace OnlineShop.Interfaces;

public interface IDatabaseConnection
{
    SqlConnection Connection { get; set; }
}