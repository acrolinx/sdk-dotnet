using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Acrolinx.Net;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using Acrolinx.Net.Acrolinx.Net;
using Acrolinx.Net.Check;
using Acrolinx.Net.Exceptions;
using Acrolinx.Net.Utils;

namespace Acrolinx.Net.Tests
{
    [TestClass]
    public class EndpointTest
    {
        private AcrolinxEndpoint endpoint = null;

        [TestMethod]
        public void TestCreate()
        {
            CreateEndpoint();
            Assert.IsNotNull(endpoint);
        }


        //[TestMethod]
        //public async Task TestInfo()
        //{
        //    CreateEndpoint();
        //    dynamic info = await endpoint.GetPlatformInformation();

        //    Assert.AreEqual("Acrolinx Core Platform", "" + info.server.name);
        //}

        [TestMethod]
        public async Task TestSso()
        {
            CreateEndpoint();
            var accessToken = await GetAccessToken();

            Assert.IsNotNull(accessToken);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(accessToken.Token));
        }

        private async Task<AccessToken> GetAccessToken()
        {
            return await endpoint.SignInWithSSO(TestEnvironment.SsoToken, TestEnvironment.Username);
        }

        [TestMethod]
        public async Task TestSsoFailDueToWrongToken()
        {
            CreateEndpoint();
            try
            {
                await endpoint.SignInWithSSO("something", TestEnvironment.Username);
            }
            catch (SsoFailedException)
            {
                return;
            }

            Assert.Fail("SsoFailedException not thrown");

        }

        [TestMethod]
        public async Task TestSsoFailDueToMissingMetadata()
        {
            CreateEndpoint();
            try
            {
                await endpoint.SignInWithSSO(TestEnvironment.SsoToken, TestEnvironment.Username + "_without_metadata");
            }
            catch (SsoFailedException)
            {
                return;
            }

            Assert.Fail("SsoFailedException not thrown");
        }

        [TestMethod]
        public async Task TestSubmitCheck()
        {
            CreateEndpoint();
            var accessToken = await GetAccessToken();

            var checkRequest = new CheckRequest()
            {
                CheckOptions = new CheckOptions()
                {
                    CheckType = CheckType.Automated,
                    ContentFormat = "TEXT"
                },
                Content = "Testdokument"
            };
            var checkResponse = await SubmitCheck(accessToken, checkRequest);

            Assert.IsNotNull(checkResponse);
            Assert.IsTrue((checkResponse.Links.ContainsKey("result")));
        }

        [TestMethod]
        public async Task TestPoll()
        {
            CreateEndpoint();
            var accessToken = await GetAccessToken();

            var checkResponse = await SubmitCheck(accessToken, new CheckRequest()
            {
                CheckOptions = new CheckOptions()
                {
                    CheckType = CheckType.Automated,
                    ContentFormat = "TEXT"
                },
                Content = "Testdokument"
            });

            Assert.IsNotNull(checkResponse);
            Assert.IsTrue((checkResponse.Links.ContainsKey("result")));

            var checkPollResponse = await endpoint.PollResult(accessToken, checkResponse);
            Assert.IsTrue(checkPollResponse.Data != null || checkPollResponse.Progress != null);
        }

        [TestMethod]
        public async Task TestCheckResponse()
        {
            CreateEndpoint();
            var accessToken = await GetAccessToken();

            var checkResponse = await SubmitCheck(accessToken, new CheckRequest()
            {
                CheckOptions = new CheckOptions()
                {
                    CheckType = CheckType.Automated,
                    ContentFormat = "TEXT"
                },
                Content = "Testdokument"
            });

            Assert.IsNotNull(checkResponse);
            Assert.IsTrue((checkResponse.Links.ContainsKey("result")));

            var checkResult = await endpoint.GetCheckResult(accessToken, checkResponse);
            Assert.IsNotNull(checkResult.Quality);
            Assert.IsNotNull(checkResult.Reports);
            Assert.AreEqual(checkResponse.Data.Id, checkResult.Id);
            Assert.AreNotSame(0, checkResult.Quality.Score);
            Assert.IsNotNull(checkResult.Quality.Status);
            Assert.IsTrue(checkResult.Reports.ContainsKey("scorecard"));
        }

        [TestMethod]
        public async Task TestContentAnalysis()
        {
            CreateEndpoint();
            var accessToken = await GetAccessToken();

            var batchId = BatchCheckIdGenerator.GenerateId("NetSDKTest");
            var result = await endpoint.GetContentAnalysisDashboard(accessToken, batchId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StartsWith(TestEnvironment.AcrolinxUrl));
        }

        private async Task<CheckResponse> SubmitCheck(AccessToken accessToken, CheckRequest checkRequest)
        {
            return await endpoint.SubmitCheck(accessToken, checkRequest);
        }

        private void CreateEndpoint()
        {
            endpoint = TestEnvironment.CreateEndpoint();
        }


    }
}
