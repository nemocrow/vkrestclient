using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VkApi.Methods;
using VkApi;

namespace Tests
{
    [TestClass]
    public class TestLongPoll : SilverlightTest
    {
        [Asynchronous]
        [TestMethod]
        public async void TestLongPollReceiveMessage()
        {
            var user = TestCommon.MainTestUser;

            // get long poll server addr
            var longPollServerInfo = (await LongPoll.GetServer().ExecuteIn(user.Session)).Data;
            Assert.IsNotNull(longPollServerInfo, "Should be able to get long poll server.");

            //
            var messageText = string.Format("A message to be received from a long poll! At {0}", DateTime.Now);

            // start polling
            var pollTask = LongPoll.Poll(longPollServerInfo, waitTimeSeconds: 5)
                .Execute();

            // meanwhile, send ourself a message
            var sendMessageResponse = 
                await Messages.Send(
                    user.UserId,
                    null,
                    messageText)
                .ExecuteIn(user.Session);
            Assert.IsTrue(sendMessageResponse.IsSuccess, "Message transfer should succeed.");

            // now, see what did our long polling call yield
            var pollResponse = await pollTask;
            Assert.IsTrue(pollResponse.IsSuccess, "Long polling should succeed.");

            var updates = pollResponse.Data.Updates.ToList(); 
            Assert.AreEqual(1, updates.Count, "Expecting just one update.");

            var updateEvent = updates.First();
            Assert.AreEqual(LongPollUpdateType.MessageAdded, updateEvent.EventType);
            var messageAddedEvent = updateEvent as LongPollUpdate.MessageAdded;
            Assert.AreEqual(messageText, messageAddedEvent.Text, "Message text should be the same.");
            Assert.AreEqual(sendMessageResponse.Data, messageAddedEvent.MessageId, "The id should be the same.");
            Assert.AreEqual(user.UserId, messageAddedEvent.FromId, "UserId should match.");

            // remove the test message
            await Messages.Delete(new[] {sendMessageResponse.Data}).ExecuteIn(user.Session);

            EnqueueTestComplete();
        }

        [Asynchronous]
        [TestMethod]
        public void TestGetLongPollServer()
        {
            LongPoll.GetServer()
            .ExecuteIn(TestCommon.MainTestUser.Session)
            .ContinueWithResultDispatched(
                (response) =>
                {
                    Assert.IsTrue(response.IsSuccess);
                    Assert.IsTrue(!string.IsNullOrEmpty(response.Data.Key));
                    Assert.IsTrue(!string.IsNullOrEmpty(response.Data.ServerUrl.ToString()));

                    Debug.WriteLine(string.Format("Got long poll server: {0}", response.Data));

                    EnqueueTestComplete();
                }
            );
        }

        [Asynchronous]
        [TestMethod]
        public void TestLongPollSimple()
        {
            LongPoll.GetServer()
            .ExecuteIn(TestCommon.MainTestUser.Session)
            .ContinueWithResultDispatched(
                (response) =>
                {
                    Assert.IsTrue(response.IsSuccess);
                    Assert.IsTrue(!string.IsNullOrEmpty(response.Data.Key));
                    Assert.IsTrue(!string.IsNullOrEmpty(response.Data.ServerUrl.ToString()));

                    Debug.WriteLine(string.Format("Got long poll server: {0}", response.Data.RawJson));

                    LongPoll.Poll(response.Data, null, 1)
                    .Execute()
                    .ContinueWithResultDispatched(
                        (response1) =>
                        {
                            Debug.WriteLine("Got long poll response: {0}", response1.Data.RawJson);

                            Assert.IsTrue(response1.IsSuccess);
                            Assert.IsTrue(response1.Data.Updates.Count() == 0);

                            EnqueueTestComplete();
                        }
                    );
                }
            );
        }
    }
}
