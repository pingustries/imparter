using Imparter.Store;

namespace Imparter.Sql
{
    public class SqlServerChannelFactory : IChannelFactory
    {
        private readonly ISqlServerOptions _settings;

        public SqlServerChannelFactory(ISqlServerOptions settings)
        {
            _settings = settings;
        }

        public IMessageQueue Get(string name)
        {
            return new SqlServerMessageQueue(_settings.MessageTypeResolver, _settings.ConnectionString, name);
        }

        public void EnsureChannelExists(string channelName)
        {
            var tableBuilder = new SqlChannelCreator(_settings.ConnectionString);
            tableBuilder.CreateIfNotExists(channelName).GetAwaiter().GetResult();
        }
    }
}
