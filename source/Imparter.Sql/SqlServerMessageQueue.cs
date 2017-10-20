using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Imparter.Store;
using Newtonsoft.Json;
using NLog;

namespace Imparter.Sql
{
    internal class SqlServerMessageQueue : IMessageQueue
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMessageTypeResolver _messageTypeResolver;
        private readonly string _connectionString;
        private readonly string _dequeueSql;
        private readonly string _enqueueSql;

        public string Name { get; }

        public SqlServerMessageQueue(IMessageTypeResolver messageTypeResolver, string connectionString, string queueName)
        {
            Name = queueName;
            _messageTypeResolver = messageTypeResolver;
            _connectionString = connectionString; 
            _dequeueSql = GetDequeueSql(queueName);
            _enqueueSql = GetEnqueueSql(queueName);
        }

        public async Task Enqueue(object message)
        {
            var type = _messageTypeResolver.GetMessageName(message.GetType());
            var serialized = Serialize(message);

            using (var connection = CreateConnection())
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _enqueueSql;
                    cmd.Parameters.Add("@data", SqlDbType.NVarChar).Value = serialized;
                    cmd.Parameters.Add("@messageType", SqlDbType.NVarChar).Value = type;
                    await cmd.ExecuteNonQueryAsync();
                }
        }

        public async Task<object> Dequeue()
        {
            MessagedataAndType result = null;
            using(var connection = CreateConnection())
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = _dequeueSql;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        result = new MessagedataAndType{Data = reader.GetString(1), Type = reader.GetString(0)};
                        _logger.Trace($"Read message from queue: '{result}'");
                    }
                        
                }
            }
            if (result == null)
                return null;
            return Deserialize(result);
        }

        private string Serialize(object message)
        {
            return JsonConvert.SerializeObject(message, typeof(object), SerializerSettings);
        }

        private object Deserialize(MessagedataAndType messagedataAndType)
        {
            var type = _messageTypeResolver.GetMessageType(messagedataAndType.Type);
            var deserialized = JsonConvert.DeserializeObject(messagedataAndType.Data, type, SerializerSettings);
            return deserialized;
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
            TypeNameHandling = TypeNameHandling.None
        };

        private static string GetEnqueueSql(string queueName)
        {
            return $"INSERT INTO {queueName}(Data, MessageType) VALUES(@data, @messageType)";
        }

        private static string GetDequeueSql(string queueName)
        {
            return string.Format(@"
DELETE TOP(1) FROM {0}
OUTPUT DELETED.MessageType, DELETED.Data
WHERE Id = (
SELECT TOP(1) Id
  FROM {0} WITH (ROWLOCK, UPDLOCK, READPAST)
ORDER BY Id)", queueName);
        }

        private class MessagedataAndType
        {
            public string Data { get; set; }
            public string Type { get; set; }

            public override string ToString()
            {
                return string.Format("Data: '{0}', Type: '{1}'", Data, Type);
            }
        }
    }
}