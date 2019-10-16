using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acrolinx.Net.Tests
{
    class TestEnvironment
    {
        public static AcrolinxEndpoint CreateEndpoint()
        {
            return new AcrolinxEndpoint(
                            AcrolinxUrl,
                            "SW50ZWdyYXRpb25EZXZlbG9wbWVudERlbW9Pbmx5");
        }

        public static string SsoToken
        {
            get
            {
                var token = Environment.GetEnvironmentVariable("ACROLINX_API_SSO_TOKEN");

                if (token == null) {
                    Trace.WriteLine("ACROLINX_API_SSO_TOKEN is unset");
                }

                return token;
            }
        }

        public static string AcrolinxUrl
        {
            get
            {
                var url = Environment.GetEnvironmentVariable("ACROLINX_API_URL");
                if (string.IsNullOrWhiteSpace(url))
                {
                    return "https://test-latest-ssl.acrolinx.com";
                    //return "https://test-ssl.acrolinx.com";
                }
                return url;
            }
        }

        public static string Username
        {
            get
            {
                var username = Environment.GetEnvironmentVariable("ACROLINX_API_USERNAME");
                if (string.IsNullOrWhiteSpace(username))
                {
                    return "sdk.net.testuser";
                }
                return username;
            }
        }
    }
}
