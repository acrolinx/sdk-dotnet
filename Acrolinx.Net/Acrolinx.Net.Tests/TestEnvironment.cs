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

using System.Diagnostics;

namespace Acrolinx.Net.Tests
{
    public class TestEnvironment
    {
        private readonly string _signature;
        private readonly string _acrolinxUrl;
        private readonly string _username;
        private readonly string _ssoToken;

        public TestEnvironment(string signature, string acrolinxUrl, string username, string ssoToken)
        {
            _signature = signature;
            _acrolinxUrl = acrolinxUrl;
            _username = username;
            _ssoToken = ssoToken;
        }

        public AcrolinxEndpoint CreateEndpoint()
        {
            return new AcrolinxEndpoint(AcrolinxUrl, _signature);
        }

        public string SsoToken
        {
            get
            {
                if (_ssoToken == null)
                {
                    Trace.WriteLine("Set the SSO token");
                }

                return _ssoToken;
            }
        }

        public string AcrolinxUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_acrolinxUrl))
                {
                    Trace.WriteLine("Set the acrolinx url");
                }
                return _acrolinxUrl;
            }
        }

        public string Username
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_username))
                {
                    Trace.WriteLine("Set the acrolinx use name");
                }
                return _username;
            }
        }
    }
}
