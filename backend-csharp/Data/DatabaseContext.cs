using MySql.Data.MySqlClient;
using System.Data;

namespace backend_csharp.Data
{
    public class DatabaseContext
    {
        private readonly string _connectionString;

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
    }
}