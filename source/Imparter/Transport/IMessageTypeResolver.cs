using System;

namespace Imparter.Store
{
    public interface IMessageTypeResolver
    {
        Type GetMessageType(string typeName);
        string GetMessageName(Type type);
    }
}