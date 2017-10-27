using Imparter.Store;

namespace Imparter.Sql
{
    public class SqlServerChannelFactory : ChannelFactoryBase
    {
        private readonly ISqlServerOptions _settings;

        public SqlServerChannelFactory(ISqlServerOptions settings) : base(settings.MessageTypeResolver, settings.MessageSerializer)
        {
            _settings = settings;
        }

        protected override IMessageQueue Get(string name)
        {
            return new SqlServerMessageQueue(_settings.ConnectionString, name);
        }

        public void EnsureChannelExists(string channelName)
        {
            var tableBuilder = new SqlChannelCreator(_settings.ConnectionString);
            tableBuilder.CreateIfNotExists(channelName).GetAwaiter().GetResult();
        }
    }
}
