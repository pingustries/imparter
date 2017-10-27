using Imparter.Store;

namespace Imparter.Sql
{
    public interface ISqlServerOptions
    {
        string ConnectionString { get; }
        IMessageTypeResolver MessageTypeResolver { get; set; }
        IMessageSerializer MessageSerializer { get; set; }
    }
}