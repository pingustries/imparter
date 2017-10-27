using System;

namespace Imparter
{
    public interface IMessageSerializer
    {
        string Serialize(object message);
        object Deserialize(Type type, string messageRaw);
    }
}