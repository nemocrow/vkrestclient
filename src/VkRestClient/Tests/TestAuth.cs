using System.Diagnostics;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VkApi;
using VkApi.Base;
using VkApi.Methods;

namespace Tests
{
    [TestClass]
    public class TestAuth : SilverlightTest
    {
        /// <summary>
        /// Send an auth request with invalid app id.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void TestInvalidClient()
        {
            // https://oauth.vk.com/token?grant_type=password&client_id=3061241&client_secret=zfF6Gdc3FSkoF1h8ZXYE&username=macofil@ukr.net&password=macmacmac

            Auth.GetToken(
                TestCommon.MainTestUser.Login, TestCommon.MainTestUser.Password,
                "yousuck", "zfF6Gdc3FSkoF1h8ZXYE")
            .ExecuteFree()
            .ContinueWithResultDispatched(
                (response) =>
                {
                    Assert.IsTrue(response.IsApiError);
                    Assert.IsTrue(response.ApiError.ExtendedError == "invalid_client");

                    EnqueueTestComplete();
                }
            );
        }

        /// <summary>
        /// Send an auth request with valid app id but while our app is not approved
        /// to use oauth yet.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void TestRestrictedApp()
        {
            Auth.GetToken(
                TestCommon.MainTestUser.Login, TestCommon.MainTestUser.Password,
                "3071802", "2HitJf058M1FiHiODMT7")
            .ExecuteFree()
            .ContinueWithResultDispatched(
                (response) =>
                {
                    Assert.IsTrue(response.IsApiError);
                    Assert.IsTrue(response.ApiError.ExtendedError == "invalid_request");

                    EnqueueTestComplete();
                }
            );
        }

        [Asynchronous]
        [TestMethod]
        public void TestVerifiedApp()
        {
            Auth.GetToken(
                TestCommon.MainTestUser.Login, TestCommon.MainTestUser.Password,
                TestCommon.AppId, TestCommon.AppKey)
            .ExecuteFree()
            .ContinueWithResultDispatched(
                (response) =>
                {
                    Assert.IsTrue(response.IsSuccess);

                    EnqueueTestComplete();
                }
            );
        }

        [Asynchronous]
        [TestMethod]
        public async void TestSignUp()
        {
            var response = await Auth.SignUp(
                "+3806617332388",
                "Johnny",
                "Cash",
                TestCommon.AppId,
                TestCommon.AppKey,
                testMode: "1")
                .ExecuteFree();

            Assert.IsTrue(response.IsSuccess, "Should be able to signup.");
            Debug.WriteLine("Signed up, and the session id is {0}", response.Data.SessionId);

            EnqueueTestComplete();
        }

        [Asynchronous]
        [TestMethod]
        public async void TestCheckPhoneBadFormat()
        {
            var response = await Auth.CheckPhone("CheeseBallz", TestCommon.AppId, TestCommon.AppKey)
                .ExecuteFree();

            Assert.IsTrue(response.IsApiError);
            Assert.IsTrue(response.ApiError.ErrorCode == ErrorCode.InvalidParameters);

            EnqueueTestComplete();
        }

        [Asynchronous]
        [TestMethod]
        public async void TestCheckPhoneUsedByAnotherUser()
        {
            var response = await Auth.CheckPhone("+380633347764", TestCommon.AppId, TestCommon.AppKey)
                .ExecuteFree();

            Assert.IsTrue(response.IsApiError);
            Assert.IsTrue(response.ApiError.ErrorCode == ErrorCode.PhoneUsedByAnotherUser);

            EnqueueTestComplete();
        }

        [Asynchronous]
        [TestMethod]
        public async void TestCheckPhoneBadNumber()
        {
            var response = await Auth.CheckPhone("+19172680029", TestCommon.AppId, TestCommon.AppKey)
                .ExecuteFree();

            Assert.IsTrue(response.IsApiError);
            Assert.IsTrue(response.ApiError.ErrorCode == ErrorCode.InvalidParameters);

            EnqueueTestComplete();
        }

    }
}
