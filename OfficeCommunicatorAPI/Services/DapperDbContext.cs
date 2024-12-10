using System.Data;
using Microsoft.Data.SqlClient;

namespace OfficeCommunicatorAPI.Services;

public class DapperDbContext
{
    public IDbConnection Connection { get; set; }
    
    public DapperDbContext(string connectionString)
    {
        Connection = new SqlConnection(connectionString);
    }
}