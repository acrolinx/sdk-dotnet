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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Acrolinx.Net.Exceptions;
using System.Net.Http.Headers;
using Acrolinx.Net.Acrolinx.Net;
using System.Threading;
using System.Reflection;
using Acrolinx.Net.Check;
using Acrolinx.Net.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Acrolinx.Net
{
    public class AcrolinxEndpoint
    {
        private string acrolinxUrl;
        private string clientLocale = "";
        private string clientSignature;
        private string clientVersion = "";
        private HttpClient client = null;

        public AcrolinxEndpoint(string acrolinxUrl, string clientSignature)
        {
            this.acrolinxUrl = acrolinxUrl;
            this.clientSignature = clientSignature;
            try
            {
                this.clientVersion = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString();
            }
            catch { }
            try
            {
                this.clientLocale = Thread.CurrentThread?.CurrentCulture?.TwoLetterISOLanguageName;
            }
            catch { }
        }

        public AcrolinxEndpoint(string acrolinxUrl, string clientSignature, string clientVersion, string clientLocale)
        {
            this.acrolinxUrl = acrolinxUrl;
            this.clientSignature = clientSignature;
            this.clientVersion = clientVersion;
            this.clientLocale = clientLocale;
        }

        protected virtual HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new HttpClient();
                }
                return client;
            }
        }

        protected virtual async Task<T> FetchDataFromApiPath<T>(string apiPath, HttpMethod method, AccessToken accessToken, Dictionary<string, string> extraHeaders, string content) where T : new()
        {
            var requestUrl = GetAbsoluteRequestUrl(apiPath);
            var request = new HttpRequestMessage(method, requestUrl);
            AddCommonHeaders(request.Headers, accessToken);
            AddExtraHeader(extraHeaders, request);
            if (content != null)
            {
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            var response = await Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            throw new LowLevelApiException(await response.Content.ReadAsStringAsync());
        }

        private string GetAbsoluteRequestUrl(string apiPath)
        {
            if (!Uri.TryCreate(apiPath, UriKind.Relative, out _))
            {
                return apiPath;
            }
            return acrolinxUrl + "/api/v1/" + apiPath;
        }

        private static void AddExtraHeader(Dictionary<string, string> extraHeaders, HttpRequestMessage request)
        {
            if (extraHeaders != null)
            {
                foreach (var entry in extraHeaders)
                {
                    request.Headers.Add(entry.Key, entry.Value);
                }
            }
        }

        protected virtual void AddCommonHeaders(HttpRequestHeaders headers, AccessToken accessToken)
        {
            if (!string.IsNullOrWhiteSpace(accessToken?.Token))
            {
                headers.Add("X-Acrolinx-Auth", accessToken.Token);
            }
            headers.Add("X-Acrolinx-Base-Url", this.acrolinxUrl);
            headers.Add("X-Acrolinx-Client-Locale", this.clientLocale);
            headers.Add("X-Acrolinx-Client", this.clientSignature + "; " + this.clientVersion);
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //public async Task<dynamic> GetPlatformInformation()
        //{
        //    dynamic obj = await FetchDataFromApiPath("", HttpMethod.Get, null, null, null);
        //    return obj.data;
        //}

        public async Task<AccessToken> SignInWithSSO(string genericToken, string username)
        {
            try
            {
                var obj = await FetchDataFromApiPath<SignInResponse>("auth/sign-ins", HttpMethod.Post,
                    null,
                    new Dictionary<string, string>(){
                    { "username", username },
                    { "password", genericToken }
                    },
                    null);
                if (obj.Links.ContainsKey("poll"))
                {
                    throw new SsoFailedException("" + obj);
                }
                return new AccessToken(obj.Data.AccessToken);
            }
            catch (LowLevelApiException e)
            {
                throw new SsoFailedException(e.Message, e);
            }
        }

        public async Task<CheckResponse> SubmitCheck(AccessToken accessToken, CheckRequest request)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            var content = JsonConvert.SerializeObject(request, serializerSettings);
            return await FetchDataFromApiPath<CheckResponse>("checking/checks", HttpMethod.Post, accessToken, null, content);
        }

        internal async Task<CheckPollResponse> PollResult(AccessToken accessToken, CheckResponse checkResponse)
        {
            var url = checkResponse.Links["result"];
            return await FetchDataFromApiPath<CheckPollResponse>(url, HttpMethod.Get, accessToken, null, null);
        }

        public async Task<CheckResult> GetCheckResult(AccessToken accessToken, CheckResponse checkResponse)
        {
            while (true)
            {
                var result = await PollResult(accessToken, checkResponse);
                if (result.Data != null)
                {
                    return result.Data;
                }
                Thread.Sleep(result.Progress.RetryAfter * 1000);
            }
        }

        public async Task<string> GetContentAnalysisDashboard(AccessToken accessToken, string batchId)
        {
            var result = await FetchDataFromApiPath<ContentAnalysisDashboard>($"checking/{batchId}/contentanalysis", HttpMethod.Get, accessToken, null, null);
            return result.Data.Links.FirstOrDefault(r => r.LinkType == "shortWithoutAccessToken")?.Link;
        }

        public async Task<CheckResult> Check(AccessToken accessToken, CheckRequest checkRequest)
        {
            var checkResponse = await SubmitCheck(accessToken, checkRequest);
            return await GetCheckResult(accessToken, checkResponse);
        }
    }
}
