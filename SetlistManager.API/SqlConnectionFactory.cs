using Microsoft.Data.SqlClient;

namespace SetlistManager.API
{
    public class SqlConnectionFactory(string connectionString)
    {
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}