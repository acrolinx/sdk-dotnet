using System.Collections.Generic;

namespace Acrolinx.Net.Check
{
    public class CheckResponse
    {
        public Dictionary<string, string> Links { get; set; }
        public CheckResponseData Data { get; set; }
    }
}
