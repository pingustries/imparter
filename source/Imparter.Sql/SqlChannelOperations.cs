using System.Threading.Tasks;
using NLog;

namespace Imparter.Sql
{
    public class SqlChannelOperations
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly SqlExecutor _sqlExecutor;

        public SqlChannelOperations(string connectionString)
        {
            _sqlExecutor = new SqlExecutor(connectionString);
        }

        public async Task CreateIfNotExists(string channelName)
        {
            string sql = $@"
IF OBJECT_ID('{channelName}', 'U') IS NULL
BEGIN
CREATE TABLE {channelName}(
    Id BIGINT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    MessageType NVARCHAR(512) NOT NULL,
    Tries INT NOT NULL,
    TimeoutUtc DateTime NULL,
    IsStopped bit NOT NULL,
    Data NVARCHAR(MAX));
    SELECT 1;
END
ELSE
BEGIN
    SELECT 0;
END
";
            var wasCreated = await _sqlExecutor.ExecuteNonQuery(sql);
            if (wasCreated)
                _logger.Info($"Queue table '{channelName}' was created");
            else
                _logger.Debug($"Queue table '{channelName}' already exists");
        }

        public async Task Purge(string channelName)
        {
            string sql = string.Format("TRUNCATE TABLE {0}", channelName);
            await _sqlExecutor.ExecuteNonQuery(sql);
        }
    }
}
