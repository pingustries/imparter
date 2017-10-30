using Imparter.Channels;
using Imparter.Store;

namespace Imparter.Transport
{
    public class MessageAndMetadata
    {
        public object Message { get; set; }
        public Metadata Metadata { get; set; }
    }
}
