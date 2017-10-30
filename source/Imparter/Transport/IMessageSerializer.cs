using System;

namespace Imparter.Transport
{
    public interface IMessageSerializer
    {
        string Serialize(object message);
        object Deserialize(Type type, string messageRaw);
    }
}