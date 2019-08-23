using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acrolinx.Net.Exceptions
{
    public class AcrolinxException : Exception
    {
        public AcrolinxException(string message, Exception innerException) : base(message, innerException) { }

        public AcrolinxException(string message) : base(message) { }

        public AcrolinxException() { }
    }
}
