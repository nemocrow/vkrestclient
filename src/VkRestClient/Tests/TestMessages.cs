using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VkApi.Methods;
using VkApi;

namespace Tests
{
    [TestClass]
    public class TestMessages : SilverlightTest
    {
        [Asynchronous]
        [TestMethod]
        public void TestGetLastFiveMessages()
        {
            const int lastMessages = 5;

            VkApi.Methods.Messages.Get(lastMessages)
            .ExecuteIn(TestCommon.MainTestUser.Session)
            .ContinueWithResultDispatched(
                (response) =>
                {
                    Assert.IsTrue(response.IsSuccess);
                    Assert.AreEqual(lastMessages, response.Data.Messages.Count());

                    Debug.WriteLine(string.Join("\n", from m in response.Data.Messages select m.Body));

                    EnqueueTestComplete();
                }
            );
        }

        [Asynchronous]
        [TestMethod]
        public async void TestSendReceiveDeleteVerify()
        {
            var messageText = string.Format("TestSendReceiveDeleteVerify test message at {0}", DateTime.Now);

            //// 1. send the message to self
            var sendResponse = 
                await VkApi.Methods.Messages.Send(
                    TestCommon.MainTestUser.AuthToken.UserId, null,
                    messageText: messageText)
                .ExecuteIn(TestCommon.MainTestUser.Session); // send as primary user

            Assert.IsTrue(sendResponse.IsSuccess);
            var sentMessageId = sendResponse.Data;

            // wait for message to deliver
            Thread.Sleep(500);

            //// 2. receive the message
            var getResponse = 
                await VkApi.Methods.Messages.Get(1)
                .ExecuteIn(TestCommon.MainTestUser.Session); // receive as primary user

            Assert.IsTrue(getResponse.IsSuccess);
            Assert.AreEqual(1, getResponse.Data.Messages.Count());
            // check that it's THE message
            var message = getResponse.Data.Messages.Last();
            Assert.AreEqual(messageText, message.Body);
            Assert.AreEqual(sentMessageId, message.Id);

            //// 3. now delete it
            var deleteResponse = 
                await VkApi.Methods.Messages.Delete(new[] {sentMessageId})
                .ExecuteIn(TestCommon.MainTestUser.Session);

            Assert.IsTrue(deleteResponse.IsSuccess);
            // was it deleted?
            Assert.IsTrue(deleteResponse.Data.AllDeleted);

            //// 4. check if the message is flagged as deleted now
            var getByIdResponse = 
                await VkApi.Methods.Messages.GetById(sentMessageId)
                .ExecuteIn(TestCommon.MainTestUser.Session);

            Assert.IsTrue(getByIdResponse.IsSuccess);
            // check if our message got the flag "deleted"
            var message1 = getByIdResponse.Data.Messages.First();
            Assert.IsTrue(message1.IsDeleted.HasValue);
            Assert.IsTrue(message1.IsDeleted.Value);

            EnqueueTestComplete();
        }


        [Asynchronous]
        [TestMethod]
        public async void TestGetDialogs()
        {
            var response = 
                await VkApi.Methods.Messages.GetDialogs()
                .ExecuteIn(TestCommon.MainTestUser.Session);

            Assert.IsTrue(response.IsSuccess);
            // @TODO what do I check here?

            EnqueueTestComplete();
        }

        [Asynchronous]
        [TestMethod]
        public async void TestRestore()
        {
            var user = TestCommon.MainTestUser;
            var message = string.Format("TestRestore message at {0}", DateTime.Now);

            // create a message
            var r = await Messages.Send(user.UserId, null, message).ExecuteIn(user.Session);
            Assert.IsTrue(r.IsSuccess, "Should send the message");
            var messageId = r.Data;

            // delete it
            var r1 = await Messages.Delete(new[] {messageId}).ExecuteIn(user.Session);
            Assert.IsTrue(r1.IsSuccess, "Should delete the message");
            Assert.IsTrue(r1.Data.AllDeleted);

            // check that it is in fact deleted
            var r2 = await Messages.GetById(messageId).ExecuteIn(user.Session);
            Assert.IsTrue(r2.IsSuccess, "Should receive the message");
            Assert.IsTrue(
                r2.Data.Messages.First().IsDeleted.HasValue && r2.Data.Messages.First().IsDeleted.Value,
                "Message should have the 'deleted' flag");

            // restore it
            var r3 = await Messages.Restore(messageId).ExecuteIn(user.Session);
            Assert.IsTrue(r3.IsSuccess, "Should restore the message");

            // check it again to see if it was restored
            var r4 = await Messages.GetById(messageId).ExecuteIn(user.Session);
            Assert.IsTrue(r4.IsSuccess);
            Assert.IsFalse(
                r4.Data.Messages.First().IsDeleted.HasValue && r4.Data.Messages.First().IsDeleted.Value,
                "Restored message should not have the 'deleted' flag");

            // delete it after all...
            var r5 = await Messages.Delete(new[] { messageId }).ExecuteIn(user.Session);
            Assert.IsTrue(r5.IsSuccess, "Should finally remove the message");
            Assert.IsTrue(r5.Data.AllDeleted);

            EnqueueTestComplete();
        }
    }
}
