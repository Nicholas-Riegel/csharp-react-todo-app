using Npgsql;
using System.Data;

namespace backend_csharp.Data
{
    public class DatabaseContext
    {
        // connection string property
        private readonly string _connectionString;

        // constructor (identified by having same name as class)
        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        // This the the method that will be used to create a connection to the database
        // Its type is "IDbConnection," which is an "interface" that NpgsqlConnection implements
        // An interface is a set of rules that a class must follow

        public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    }
}
