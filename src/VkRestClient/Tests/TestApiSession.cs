using System;
using System.Collections.Generic;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using VkApi.JsonTypes;
using VkApi;

namespace Tests
{
    [TestClass]
    public class TestApiSession : SilverlightTest
    {
        [Asynchronous]
        [TestMethod]
        public void TestApiError()
        {
            // https://api.vk.com/method/getProfiles?uid=66748&access_token=533bacf01e11f55b536a565b57531ac114461ae8736d6506a3

            // should return:
            // {"error":{"error_code":5,"error_msg":"User authorization failed: user revoke access for this token.","request_params":[{"key":"oauth","value":"1"},{"key":"method","value":"getProfiles"},{"key":"uid","value":"66748"},{"key":"access_token","value":"533bacf01e11f55b536a565b57531ac114461ae8736d6506a3"}]}}            
            VkApi.Methods.Friends.Get()
            .ExecuteIn(TestCommon.NewSession("invalid_token"))
            .ContinueWithResultDispatched(
                (response) =>
                    {
                        Assert.IsTrue(response.IsApiError);
                        Assert.IsTrue(response.ApiError.ErrorCode == VkApi.Base.ErrorCode.UserAuthorizationFailed);

                        EnqueueTestComplete();
                    }
            );
        }

        [Asynchronous]
        [TestMethod]
        public void TestGetProfiles()
        {
            new VkApi.Methods.Base.SessionApiRequest<List<VkUserProfile>>(
                "getProfiles",
                Method.GET,
                new VkApi.Methods.Base.ParameterDict { { "uid", "66748" } })
            .ExecuteIn(TestCommon.MainTestUser.Session)
            .ContinueWithResultDispatched(
                (response) =>
                    {
                        Assert.IsTrue(response.IsSuccess);
                        Assert.AreEqual(1, response.Data.Count);

                        EnqueueTestComplete();
                    }
            );
        }

    }
}
