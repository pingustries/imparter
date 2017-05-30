using Imparter.Store;

namespace Imparter.Sql
{
    public class SqlServerMessageQueueFactory : IMessageQueueFactory
    {
        private readonly string _connectionString;

        public SqlServerMessageQueueFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IMessageQueue Get(string name)
        {
            return new SqlServerMessageQueue(_connectionString, name);
        }
    }
}
