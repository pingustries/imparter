using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Imparter.Sql
{
    internal class SqlExecutor
    {
        private readonly string _connectionString;

        public SqlExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (var connection = CreateConnection())
            using (var cmd = connection.CreateCommand())
            {
                cmd.Parameters.AddRange(parameters);
                cmd.CommandText = sql;
                var result  = await cmd.ExecuteScalarAsync();
                return ((int?) result) == 1;
            }
        }

        private SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}