using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acrolinx.Net.Exceptions
{
    public class LowLevelApiException : AcrolinxException
    {
        public LowLevelApiException(string message) : base(message) { }
    }
}
