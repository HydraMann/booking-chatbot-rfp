using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Testing;

using Microsoft.Bot.Connector;

using BookingBotRFT.Dialogs;
using BookingBotRFT;

namespace BookingDialogTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task BookingTest()
        {
            var sut = new MainDialog();
            var testClient = new DialogTestClient(Channels.Msteams, sut);

            var reply = await testClient.SendActivityAsync<IMessageActivity>("hi");
            Assert.AreEqual(null, reply.Text);

            reply = await testClient.SendActivityAsync<IMessageActivity>("Booking");
            Assert.AreEqual("Please type in your name.", reply.Text);

            reply = await testClient.SendActivityAsync<IMessageActivity>("James");
            Assert.AreEqual("Please type in your email.", reply.Text);

            reply = await testClient.SendActivityAsync<IMessageActivity>("james@gmail.com");
            Assert.AreEqual("Please type in where you want to travel.", reply.Text);

            reply = await testClient.SendActivityAsync<IMessageActivity>("Atlanta Hotel");
            Assert.AreEqual("Please type in when you want to travel.", reply.Text);

            var Results = (MainDialog)testClient.DialogTurnResult.Result;
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var sut = new MainDialog();
            var testClient = new DialogTestClient(Channels.Msteams, sut);

            var reply = await testClient.SendActivityAsync<IMessageActivity>("hi");
            Assert.AreEqual(null, reply.Text);

            reply = await testClient.SendActivityAsync<IMessageActivity>("Delete");
            Assert.AreEqual("Please give the booking id", reply.Text);

            reply = await testClient.SendActivityAsync<IMessageActivity>("5461564156");
            Assert.AreEqual(null, reply.Text);

            var Results = (MainDialog)testClient.DialogTurnResult.Result;
        }
    }
}
