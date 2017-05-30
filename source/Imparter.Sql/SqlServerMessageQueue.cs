using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Imparter.Store;
using Newtonsoft.Json;

namespace Imparter.Sql
{
    public class SqlServerMessageQueue : IMessageQueue
    {
        private readonly string _connectionString;
        private readonly string _dequeueSql;
        private readonly string _enqueueSql;

        public SqlServerMessageQueue(string connectionString, string queueName)
        {
            _connectionString = connectionString; 
            _dequeueSql = string.Format(@"
DELETE TOP(1) FROM {0}
OUTPUT DELETED.Data
WHERE Id = (
SELECT TOP(1) Id
  FROM {0} WITH (ROWLOCK, UPDLOCK, READPAST)
ORDER BY Id)", queueName);

            _enqueueSql = $"INSERT INTO {queueName}(Data) VALUES(@data)";
        }

        public async Task Enqueue(IMessage message)
        {
            var serialized = Serialize(message);

            using (var connection = CreateConnection())
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _enqueueSql;
                    cmd.Parameters.Add("@data", SqlDbType.NVarChar).Value = serialized;
                    await cmd.ExecuteNonQueryAsync();
                }
        }

        public async Task<IMessage> Dequeue()
        {
            string result = null;
            using(var connection = CreateConnection())
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = _dequeueSql;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                        result = reader.GetString(0);
                }
            }
            if (string.IsNullOrEmpty(result))
                return null;
            return Deserialize(result);
        }

        public static string GetCreateTableSql(string queueName)
        {
            return string.Format(@"
CREATE TABLE {0}(
    Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    Data NVARCHAR(MAX));
");
        }

        private string Serialize(IMessage message)
        {
            return JsonConvert.SerializeObject(message, typeof(IMessage), SerializerSettings);
        }

        private IMessage Deserialize(string raw)
        {
            return JsonConvert.DeserializeObject<IMessage>(raw, SerializerSettings);
        }

        private SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        };
    }
}