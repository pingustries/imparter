using System;

namespace Imparter.Transport
{
    public class Metadata
    {
        public DateTime? TimeoutUtc { get; set; }
        public int Tries { get; set; }
        public bool IsStopped { get; set; }
    }
}