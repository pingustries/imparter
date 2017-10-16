using System;

namespace Imparter.Sql
{
    internal class SimpleMessageTypeResolver : IMessageTypeResolver
    {
        public Type GetMessageType(string typeName)
        {
            return Type.GetType(typeName);
        }

        public string GetMessageName(Type type)
        {
            return type.AssemblyQualifiedName;
        }
    }
}
