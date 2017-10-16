using System;

namespace Imparter.Sql
{
    public interface IMessageTypeResolver
    {
        Type GetMessageType(string typeName);
        string GetMessageName(Type type);
    }
}