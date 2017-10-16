namespace Imparter.Sql
{
    public interface ISqlServerOptions
    {
        string ConnectionString { get; }
        IMessageTypeResolver MessageTypeResolver { get; set; }
    }
}