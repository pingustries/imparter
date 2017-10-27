using System;
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
        private readonly string _connectionString;
        private readonly string _dequeueSql;
        private readonly string _enqueueSql;

        public string Name { get; }

        public SqlServerMessageQueue(string connectionString, string queueName)
        {
            Name = queueName;
            _connectionString = connectionString; 
            _dequeueSql = GetDequeueSql(queueName);
            _enqueueSql = GetEnqueueSql(queueName);
        }

        public async Task Enqueue(MessageAndMetadata messageAndMetadata)
        {
            using (var connection = CreateConnection())
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _enqueueSql;
                    cmd.Parameters.Add("@data", SqlDbType.NVarChar).Value = messageAndMetadata.MessageRaw;
                    cmd.Parameters.Add("@messageType", SqlDbType.NVarChar).Value = messageAndMetadata.Metadata.MessageType;
                    cmd.Parameters.Add("@timeoutUtc", SqlDbType.DateTime).Value = messageAndMetadata.Metadata.TimeoutUtc.HasValue ? (object)messageAndMetadata.Metadata.TimeoutUtc : DBNull.Value;
                    cmd.Parameters.Add("@tries", SqlDbType.Int).Value = messageAndMetadata.Metadata.Tries;
                    cmd.Parameters.Add("@isStopped", SqlDbType.Bit).Value = messageAndMetadata.Metadata.IsStopped;

                    await cmd.ExecuteNonQueryAsync();
                }
        }

        public async Task<MessageAndMetadata> Dequeue()
        {
            using(var connection = CreateConnection())
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = _dequeueSql;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        var result = Map(reader);
                        _logger.Trace($"Read message from queue: '{result}'");
                        return result;
                    }
                }
            }
            return null;
        }


        private SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }


        private MessageAndMetadata Map(SqlDataReader reader)
        {
            var type = reader.GetString(0);
            var timeOutUtc = reader.IsDBNull(1) ? (DateTime?) null : reader.GetDateTime(1);
            var tries = reader.GetInt32(2);
            var isStopped = reader.GetBoolean(3);
            var data = reader.GetString(4);
            return new MessageAndMetadata
            {
                MessageRaw = data,
                Metadata = new Metadata {IsStopped = isStopped, TimeoutUtc = timeOutUtc, Tries = tries, MessageType = type}
            };
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
    ORDER BY TimeoutUTC DESC
)
DELETE FROM cte
OUTPUT 
    DELETED.MessageType, DELETED.TimeoutUtc, DELETED.Tries, DELETED.IsStopped, DELETED.Data", queueName);
        }
    }
}