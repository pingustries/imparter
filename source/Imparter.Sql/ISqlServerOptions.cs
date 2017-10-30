using Imparter.Transport;

namespace Imparter.Sql
{
    public interface ISqlServerOptions
    {
        string ConnectionString { get; }
        ITransportTranslator TransportTranslator { get; set; }
    }
}