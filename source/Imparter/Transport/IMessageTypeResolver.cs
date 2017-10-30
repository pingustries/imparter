using System;

namespace Imparter.Transport
{
    public interface IMessageTypeResolver
    {
        Type GetMessageType(string typeName);
        string GetMessageName(Type type);
    }
}