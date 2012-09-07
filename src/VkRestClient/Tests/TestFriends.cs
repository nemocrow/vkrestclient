using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VkApi;

namespace Tests
{
    [TestClass]
    public class TestFriends : SilverlightTest
    {
        [Asynchronous]
        [TestMethod]
        public void TestGet()
        {
            VkApi.Methods.Friends.Get()
            .ExecuteIn(TestCommon.MainTestUser.Session)
            .ContinueWithResultDispatched(
                (response) =>
                {
                    Assert.IsTrue(response.IsSuccess);

                    Debug.WriteLine(string.Join("\n", from m in response.Data select m.LastName));

                    EnqueueTestComplete();
                }
            );
        }

    }
}
