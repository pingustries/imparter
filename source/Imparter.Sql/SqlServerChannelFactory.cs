using System.Threading.Tasks;
using Imparter.Transport;

namespace Imparter.Sql
{
    public class SqlServerChannelFactory : ChannelFactoryBase
    {
        private readonly ISqlServerOptions _settings;

        public SqlServerChannelFactory(ISqlServerOptions settings)
        {
            _settings = settings;
        }

        protected override IMessageQueue Get(string name)
        {
            return new SqlServerMessageQueue(_settings.ConnectionString, name, _settings.TransportTranslator);
        }

        public async Task EnsureChannelExists(string channelName)
        {
            var tableOperations = new SqlChannelOperations(_settings.ConnectionString);
            await tableOperations.CreateIfNotExists(channelName);
        }

        public async Task PurgeChannel(string channelName)
        {
            var tableOperations = new SqlChannelOperations(_settings.ConnectionString);
            await tableOperations.Purge(channelName);
        }
    }
}
