namespace Imparter.Transport
{
    public interface ITransportTranslator
    {
        object FromTransport(string serializedMessage, string serializedType);
        string SerializeForTransport(object message);
        string SerialzeTypeForTransport(object message);
    }
}
