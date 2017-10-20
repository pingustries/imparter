using System.Threading.Tasks;
using NLog;

namespace Imparter.Sql
{
    public class SqlChannelCreator
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly SqlExecutor _sqlExecutor;

        public SqlChannelCreator(string connectionString)
        {
            _sqlExecutor = new SqlExecutor(connectionString);
        }

        public async Task CreateIfNotExists(string channelName)
        {
            string sql = $@"
IF OBJECT_ID('{channelName}', 'U') IS NULL
BEGIN
CREATE TABLE {channelName}(
    Id INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    MessageType NVARCHAR(512) NOT NULL,
    Tries INT NOT NULL DEFAULT 1,
    Timeout DateTime NULL,
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
    }
}
