using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Imparter.Sql
{
    public class SqlExecutor
    {
        private readonly ISqlServerSettings _settings;

        public SqlExecutor(ISqlServerSettings settings)
        {
            _settings = settings;
        }

        public async Task ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (var connection = CreateConnection())
            using (var cmd = connection.CreateCommand())
            {
                cmd.Parameters.AddRange(parameters);
                cmd.CommandText = sql;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(_settings.ConnectionString);
            connection.Open();
            return connection;
        }
    }
}