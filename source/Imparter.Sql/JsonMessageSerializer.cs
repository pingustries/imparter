using System;
using Newtonsoft.Json;

namespace Imparter.Sql
{
    internal class JsonMessageSerializer : IMessageSerializer
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None
        };

        public string Serialize(object message)
        {
            return JsonConvert.SerializeObject(message, typeof(object), SerializerSettings);
        }

        public object Deserialize(Type type, string messageRaw)
        {
            var deserialized = JsonConvert.DeserializeObject(messageRaw, type, SerializerSettings);
            return deserialized;
        }
    }
}