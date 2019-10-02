using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acrolinx.Net.Internal
{
    public class SignInResponse
    {
        public SignInSuccess Data { get; set; }
        public Dictionary<string, string> Links { get; set; }

    }
}
