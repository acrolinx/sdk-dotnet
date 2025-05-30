﻿/*
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

using System.Net;
using System.Net.Http.Headers;
using Acrolinx.Net.Acrolinx.Net;
using Acrolinx.Net.Check;
using Acrolinx.Net.Exceptions;
using Acrolinx.Net.Utils;
using Moq;
using Moq.Protected;

namespace Acrolinx.Net.Tests
{
    [TestClass]
    public class EndpointTest : TestBase
    {
        private AcrolinxEndpoint endpoint = null;

        [TestInitialize] // Runs before each test
        public void Init()
        {
            endpoint = TestEnvironment.CreateEndpoint();
        }

        [TestMethod]
        public void TestCreate()
        {
            Assert.IsNotNull(endpoint);
        }

        [TestMethod]
        public async Task TestSso()
        {
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
        [Timeout(3000)]
        public void TestSignInInteractiveHasPollUrl()
        {
            var openUrlMock = new Mock<AcrolinxEndpoint.OpenUrl>();
            var signIn = endpoint.SignInInteractive(openUrlMock.Object);
            Thread.Sleep(2000);
            openUrlMock.Verify(_ => _(It.IsAny<Uri>()), Times.Once);
        }

        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(SignInFailedException), "Timeout")]
        public async Task TestSignInInteractiveTimeout()
        {
            await endpoint.SignInInteractive((url) => { }, new TimeSpan(0, 0, 1), null);
        }

        [TestMethod]
        [Timeout(1 * 60 * 1000)]
        [Ignore]
        public async Task ManualTestSignInInteractive()
        {
            AccessToken accessToken = await endpoint.SignInInteractive((url) =>
            {
                System.Diagnostics.Trace.WriteLine("Sign in manually in browser please: " + url);
                System.Diagnostics.Process.Start(url.ToString());
            });
            System.Diagnostics.Trace.WriteLine("The AccessToken is: " + accessToken.Token);
            Assert.IsTrue(!string.IsNullOrEmpty(accessToken.Token));
        }


        [TestMethod]
        public async Task TestSubmitCheck()
        {
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
        [Timeout(4 * 1000)]
        public async Task TestSignInInteractiveWithTokenUsesTokenWithoutUserInteraction()
        {
            var accessToken = await GetAccessToken();

            bool interactiveCalled = false;
            AccessToken accessToken2 = await endpoint.SignInInteractive((_) => { interactiveCalled = true; }, accessToken);

            Assert.IsFalse(interactiveCalled);
            Assert.AreEqual(accessToken?.Token, accessToken2?.Token);
        }

        [TestMethod]
        [Timeout(10 * 1000)]
        public async Task TestSignInInteractiveWithInvalidTokenFallsBackToInteractive()
        {
            var accessToken = new AccessToken("invalid");

            bool interactiveCalled = false;
            try
            {
                AccessToken accessToken2 = await endpoint.SignInInteractive((_) => { interactiveCalled = true; }, new TimeSpan(0, 0, 2), accessToken);
            }
            catch (SignInFailedException e)
            {
                Assert.AreEqual("Timeout", e.Message);
            }

            Assert.IsTrue(interactiveCalled);
        }

        [TestMethod]
        [Timeout(10 * 1000)]
        public async Task TestSignInInteractiveWithInvalidTokenFallsBackToInteractive2()
        {
            bool interactiveCalled = false;
            try
            {
                AccessToken accessToken2 = await endpoint.SignInInteractive((_) => { interactiveCalled = true; }, new TimeSpan(0, 0, 4), null);
            }
            catch (SignInFailedException e)
            {
                Assert.AreEqual("Timeout", e.Message);
            }

            Assert.IsTrue(interactiveCalled);
        }

        [TestMethod]
        public async Task TestPoll()
        {
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
            var accessToken = await GetAccessToken();

            var checkResponse = await SubmitCheck(accessToken, new CheckRequest()
            {
                CheckOptions = new CheckOptions()
                {
                    CheckType = CheckType.Automated,
                    ContentFormat = "TEXT",
                    // Adjust following id as per your test instance. 
                    GuidanceProfileId = "85d7c67b-9daf-3f95-9089-68eeadca6914"
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
            var accessToken = await GetAccessToken();

            var batchId = BatchCheckIdGenerator.GenerateId("NetSDKTest");
            var result = await endpoint.GetContentAnalysisDashboard(accessToken, batchId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StartsWith(TestEnvironment.AcrolinxUrl));
        }

        [TestMethod]
        public async Task TestGetCapabilities()
        {
            var accessToken = await GetAccessToken();
            var result = await endpoint.GetCapabilities(accessToken);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
        }

        [TestMethod]
        public async Task TestCheckResultIssues()
        {
            var accessToken = await GetAccessToken();

            var checkResponse = await SubmitCheck(accessToken, new CheckRequest()
            {
                CheckOptions = new CheckOptions()
                {
                    CheckType = CheckType.Automated,
                    ContentFormat = "TEXT"
                },
                Content = "Thiss texxt containss intentional errorss!"
            });

            Assert.IsNotNull(checkResponse);
            Assert.IsTrue(condition: (checkResponse.Links.ContainsKey("result")));

            var checkResult = await endpoint.GetCheckResult(accessToken, checkResponse);
            Assert.IsNotNull(checkResult.Issues);
            Assert.IsTrue(checkResult.Issues.Count > 0);
            Assert.IsNotNull(checkResult.Issues[0].PositionalInformation);
            Assert.IsNotNull(checkResult.Issues[0].DisplaySurface);
            Assert.IsNotNull(checkResult.Issues[0].GuidanceHtml);
            Assert.IsNotNull(checkResult.Issues[0].GoalId);
            Assert.IsNotNull(checkResult.Issues[0].Suggestions);
        }

        [TestMethod]
        public async Task TestGetAccessToken_WithEndpointUrlVariations()
        {
            var acrolinxEndpointUrl = TestEnvironment.AcrolinxUrl;
            var urlVariations = new List<string>
            {
                acrolinxEndpointUrl.TrimEnd('/'),
                acrolinxEndpointUrl.TrimEnd('/') + "/",
                acrolinxEndpointUrl.TrimEnd('/') + "//"
            };

            Assert.IsTrue(urlVariations.Count == 3);
            Assert.IsFalse(urlVariations[0].EndsWith("/"));
            Assert.IsTrue(urlVariations[1].EndsWith("/"));
            Assert.IsTrue(urlVariations[2].EndsWith("//"));

            foreach (var url in urlVariations)
            {
                var endpoint = new AcrolinxEndpoint(url, TestEnvironment.Signature);
                var accessToken = await endpoint.SignInWithSSO(TestEnvironment.SsoToken, TestEnvironment.Username);

                Assert.IsNotNull(accessToken);
            }
        }

        [TestMethod]
        public async Task TestSignInWithSSO_EncodesHeadersCorrectly()
        {
            // Arrange the Mock
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"data\": {\"accessToken\": \"testToken\"}, \"links\": {\"self\": \"https://acrolinx.com\"}}"),
                })
                .Verifiable();

            // Create the Acrolinx Endpoint with the Mock
            var httpClient = new HttpClient(handlerMock.Object);
            var endpoint = new AcrolinxEndpoint(TestEnvironment.AcrolinxUrl, TestEnvironment.Signature, httpClient);

            var username = "abcd äöüß";
            var genericToken = "!#$%&<=>@?";

            // Sign in using SSO
            await endpoint.SignInWithSSO(genericToken, username);

            // Assert the result
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post
                    && req.Headers.Contains("username")
                    && req.Headers.Contains("password")
                    && UrlDecodeHeader(req.Headers, "username") == username
                    && UrlDecodeHeader(req.Headers, "password") == genericToken),
                ItExpr.IsAny<CancellationToken>());
        }

        private string UrlDecodeHeader(HttpRequestHeaders headers, string headerValue)
        {
            return Uri.UnescapeDataString(headers.GetValues(headerValue).First());
        }

        private async Task<CheckResponse> SubmitCheck(AccessToken accessToken, CheckRequest checkRequest)
        {
            return await endpoint.SubmitCheck(accessToken, checkRequest);
        }
    }
}
