/*
 * Copyright 2019-present Acrolinx GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;

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
                    return "https://test-ssl.acrolinx.com";
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
