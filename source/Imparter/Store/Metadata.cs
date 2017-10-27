using System;

namespace Imparter.Store
{
    public class Metadata
    {
        public string MessageType { get; set; }
        public DateTime? TimeoutUtc { get; set; }
        public int Tries { get; set; }
        public bool IsStopped { get; set; }
    }
}