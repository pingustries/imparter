using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Imparter.Transport;
using NLog;

namespace Imparter.Sql
{
    internal class SqlServerMessageQueue : IMessageQueue
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _connectionString;
        private readonly ITransportTranslator _transportTranslator;
        private readonly string _dequeueSql;
        private readonly string _enqueueSql;

        public string Name { get; }

        public SqlServerMessageQueue(string connectionString, string queueName, ITransportTranslator transportTranslator)
        {
            Name = queueName;
            _connectionString = connectionString;
            _transportTranslator = transportTranslator;
            _dequeueSql = GetDequeueSql(queueName);
            _enqueueSql = GetEnqueueSql(queueName);
        }

        public async Task Enqueue(object message, Metadata metadata = null)
        {
            var messageSerialized = _transportTranslator.SerializeForTransport(message);
            var typeSerialized  = _transportTranslator.SerialzeTypeForTransport(message);
            metadata = metadata ?? new Metadata();

            using (var connection = CreateConnection())
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = _enqueueSql;
                cmd.Parameters.Add("@data", SqlDbType.NVarChar).Value = messageSerialized;
                cmd.Parameters.Add("@messageType", SqlDbType.NVarChar).Value = typeSerialized;
                cmd.Parameters.Add("@timeoutUtc", SqlDbType.DateTime).Value = metadata.TimeoutUtc.HasValue ? (object)metadata.TimeoutUtc : DBNull.Value;
                cmd.Parameters.Add("@tries", SqlDbType.Int).Value = metadata.Tries;
                cmd.Parameters.Add("@isStopped", SqlDbType.Bit).Value = metadata.IsStopped;

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<MessageAndMetadata> Dequeue()
        {
            Metadata metadata = null;
            string messageSerialized = null;
            string messageTypeSerialized = null;
            using (var connection = CreateConnection())
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = _dequeueSql;
                cmd.Parameters.Add("@Now", SqlDbType.DateTime).Value = DateTime.UtcNow;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        metadata = GetMetadata(reader);
                        messageTypeSerialized = reader.GetString(0);
                        messageSerialized = reader.GetString(4);
                        _logger.Trace($"Read message from queue: '{messageSerialized}'");
                    }
                }
            }

            if (messageSerialized == null || messageTypeSerialized == null)
                return null;
            return new MessageAndMetadata
            {
                Metadata = metadata,
                Message = _transportTranslator.FromTransport(messageSerialized, messageTypeSerialized)
            };
        }


        private SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }


        private Metadata GetMetadata(SqlDataReader reader)
        {
            var timeOutUtc = reader.IsDBNull(1) ? (DateTime?)null : reader.GetDateTime(1);
            var tries = reader.GetInt32(2);
            var isStopped = reader.GetBoolean(3);
            return new Metadata {IsStopped = isStopped, TimeoutUtc = timeOutUtc, Tries = tries};

        }

        private static string GetEnqueueSql(string queueName)
        {
            return $"INSERT INTO {queueName}(Data, MessageType, TimeoutUtc, Tries, IsStopped) VALUES(@data, @messageType, @timeoutUtc, @tries, @isStopped)";
        }

        private static string GetDequeueSql(string queueName)
        {
            return string.Format(@"
WITH cte AS(
    SELECT TOP(1) MessageType, Data, TimeoutUtc, Tries, IsStopped 
    FROM {0} WITH (ROWLOCK, READPAST)
    WHERE (TimeoutUTC IS NULL OR TimeoutUTC < @Now) AND IsStopped = 0
    ORDER BY TimeoutUTC DESC
)
DELETE FROM cte
OUTPUT 
    DELETED.MessageType, DELETED.TimeoutUtc, DELETED.Tries, DELETED.IsStopped, DELETED.Data", queueName);
        }
    }
}