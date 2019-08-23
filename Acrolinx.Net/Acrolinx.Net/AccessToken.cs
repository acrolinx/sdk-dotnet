using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acrolinx.Net.Acrolinx.Net
{
    public class AccessToken
    {

        public AccessToken(string token)
        {
            Token = token;
        }

        public string Token
        {
            get;internal set;
        }
    }
}
