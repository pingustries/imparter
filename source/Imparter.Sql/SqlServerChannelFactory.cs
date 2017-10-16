using Imparter.Store;

namespace Imparter.Sql
{
    public class SqlServerChannelFactory : IChannelFactory
    {
        private readonly ISqlServerSettings _settings;

        public SqlServerChannelFactory(ISqlServerSettings settings)
        {
            _settings = settings;
        }

        public IMessageQueue Get(string name)
        {
            return new SqlServerMessageQueue(_settings.ConnectionString, name);
        }

        public void EnsureChannelExists(string channelName)
        {
            var tableBuilder = new SqlChannelCreator(_settings);
            tableBuilder.CreateIfNotExists(channelName).GetAwaiter().GetResult();
        }
    }
}
