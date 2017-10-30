using Imparter.Transport;

namespace Imparter.Sql
{
    public class SqlServerOptions : ISqlServerOptions
    {
        public string ConnectionString { get; }
        public ITransportTranslator TransportTranslator { get; set; }

        public SqlServerOptions(string connectionString)
        {
            ConnectionString = connectionString;
            TransportTranslator = new SerializingTransportTranslator(new SimpleMessageTypeResolver(), new JsonMessageSerializer());
        }
    }
}