using System;
using System.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VkApi.Methods;
using VkApi;

namespace Tests
{
    [TestClass]
    public class TestUsers : SilverlightTest
    {
        [Asynchronous]
        [TestMethod]
        public async void TestGet()
        {
            var user = TestCommon.MainTestUser;
            var r = await Users.Get(new[] {user.UserId}).ExecuteIn(user.Session);
            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(user.UserId, r.Data.First().Id);

            EnqueueTestComplete();
        }
    }
}
