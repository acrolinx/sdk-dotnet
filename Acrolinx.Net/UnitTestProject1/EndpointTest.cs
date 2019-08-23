using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Acrolinx.Net;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using Acrolinx.Net.Exceptions;

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


        [TestMethod]
        public async Task TestInfo()
        {
            CreateEndpoint();
            dynamic info = await endpoint.GetPlatformInformation();
            
            Assert.AreEqual("Acrolinx Core Platform", "" + info.server.name);
        }

        [TestMethod]
        public async Task TestSso()
        {
            CreateEndpoint();
            var authentication = await endpoint.SignInWithSSO(TestEnvironment.SsoToken, TestEnvironment.Username);

            Assert.IsNotNull(authentication);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(authentication.Token));
        }

        [TestMethod]
        public async Task TestSsoFailDueToWrongToken()
        {
            CreateEndpoint();
            try
            {
            await endpoint.SignInWithSSO("something", TestEnvironment.Username);
            }
            catch(SsoFailedException)
            {
                return;
            }

            Assert.Fail("SsoFailedException not thrown");

        }

        [TestMethod]
        [Ignore]
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

        private void CreateEndpoint()
        {
            endpoint = TestEnvironment.CreateEndpoint();
        }
    }
}
