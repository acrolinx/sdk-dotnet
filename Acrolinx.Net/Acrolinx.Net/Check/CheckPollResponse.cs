using System.Collections.Generic;
using Acrolinx.Net.Internal;

namespace Acrolinx.Net.Check
{
    public class CheckPollResponse
    {
        public Dictionary<string, string> Links { get; set; }
        public Progress Progress { get; set; }
        public CheckResult Data { get; set; }
    }
}
