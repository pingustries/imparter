using System;

namespace Imparter.Transport
{
    public class SimpleMessageTypeResolver : IMessageTypeResolver
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
