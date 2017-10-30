using Imparter.Store;

namespace Imparter.Transport
{
    public interface ITransportTranslator
    {
        object FromTransport(string serialized, Metadata metadata);
        string PrepareForTransport(object message);
        Metadata PrepareMetaDataForTransport(object message);
    }
}
