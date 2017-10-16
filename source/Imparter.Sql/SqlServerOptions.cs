namespace Imparter.Sql
{
    public class SqlServerOptions : ISqlServerOptions
    {
        public string ConnectionString { get; }
        public IMessageTypeResolver MessageTypeResolver { get; set; }

        public SqlServerOptions(string connectionString)
        {
            ConnectionString = connectionString;
            MessageTypeResolver = new SimpleMessageTypeResolver();
        }
    }
}