using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acrolinx.Net.Exceptions
{
    public class SsoFailedException : AcrolinxException
    {
        public SsoFailedException(string message) : base(message) { }
        public SsoFailedException(string message, Exception e) : base(message, e) { }
    }
}
