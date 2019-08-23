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
using Acrolinx.Net.Internal;
using Newtonsoft.Json.Linq;

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

        protected virtual async Task<JObject> FetchDataFromApiPath(String apiPath, HttpMethod method, AccessToken accessToken, Dictionary<string, string> extraHeaders, string content)
        {
            var request = new HttpRequestMessage(method, acrolinxUrl + "/api/v1/" + apiPath);
            AddCommonHeaders(request.Headers, accessToken);
            AddExtraHeader(extraHeaders, request);
            if (content != null)
            {
                //request.Content. content;
            }

            var response = await Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                //return (await response.Content.ReadAsAsync<SuccessResponse<T>>()).data;
                var json = await response.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(json);
                return obj;
            }
            throw new LowLevelApiException(await response.Content.ReadAsStringAsync());
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

        public async Task<dynamic> GetPlatformInformation()
        {
            dynamic obj = await FetchDataFromApiPath("", HttpMethod.Get, null, null, null);
            return obj.data;
        }

        public async Task<AccessToken> SignInWithSSO(string genericToken, string username)
        {
            try
            {

                JObject obj = await FetchDataFromApiPath("auth/sign-ins", HttpMethod.Post,
                    null,
                    new Dictionary<string, string>(){
                    { "username", username },
                    { "password", genericToken }
                    },
                    null);
                if (!string.IsNullOrWhiteSpace(((dynamic)((dynamic)obj).links).poll))
                {
                    throw new SsoFailedException("" + obj);
                }
                return new AccessToken("" + ((dynamic)obj).data.accessToken);
            }
            catch (LowLevelApiException e)
            {
                throw new SsoFailedException(e.Message, e);
            }
        }
    }
}
