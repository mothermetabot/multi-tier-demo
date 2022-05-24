using Common.Structs;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Application1.Services;

namespace Tests.UnitTests.Application1
{
    internal class RealTimeServiceTests
    {
        private Mock<IHubProxy> _mockHubProxy;

        private RealTimeService _realTimeService;

        private Mock<IObserver<HubConnectionState>> _mockStateObserver;

        private Mock<IObserver<string>> _mockMessageObserver;

        private IDisposable _stateObserverSubscription;
        private IDisposable _messageObserverSubscription;

        [SetUp]
        public void Setup()
        {
            _mockHubProxy = new();
            _realTimeService = new(_mockHubProxy.Object);
            _mockStateObserver = new();
            _mockMessageObserver = new();

            _stateObserverSubscription = _realTimeService.MessageObservable.Subscribe(_mockMessageObserver.Object);

            _messageObserverSubscription = _realTimeService.ConnectionStateObservable.Subscribe(_mockStateObserver.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _stateObserverSubscription.Dispose();
            _messageObserverSubscription.Dispose();
        }


        [Test]
        public async Task Broadcast()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _realTimeService.Broadcast(null));

            var message = "test";
            await _realTimeService.Broadcast(message);

            _mockHubProxy.Verify(h =>
                h.SendAsync(RemoteProcedureCall.SEND_BROADCAST, message, CancellationToken.None),
                Times.Once);
        }

        [Test]
        public async Task Connect_SubscribesToMessages()
        {
            var url = "testUrl";
            await _realTimeService.Connect(url);

            _mockHubProxy.Verify(h =>
                h.On(RemoteProcedureCall.RECEIVE_MESSAGE, It.IsAny<Func<string, Task>>()),
                Times.Once);
        }

        [Test]
        public async Task Connect_NotifiesConnectionState()
        {
            _mockHubProxy.SetupGet(h => h.State)
                .Returns(HubConnectionState.Connected);

            var url = "testUrl";
            await _realTimeService.Connect(url);

            _mockHubProxy.VerifyAdd(h => h.Closed += It.IsAny<Func<Exception?, Task>?>(),
                Times.Once);
            _mockHubProxy.VerifyAdd(h => h.Reconnecting += It.IsAny<Func<Exception?, Task>?>(),
                Times.Once);
            _mockHubProxy.VerifyAdd(h => h.Reconnected += It.IsAny<Func<string?, Task>?>(),
                Times.Once);

            _mockStateObserver.Verify(m => m.OnNext(HubConnectionState.Connected), Times.Once);
        }

        [Test]
        public async Task DisposeAsync()
        {
            await _realTimeService.DisposeAsync();

            _mockHubProxy.Verify(m => m.StopAsync(CancellationToken.None), Times.Once);
            _mockHubProxy.Verify(m => m.DisposeAsync(), Times.Once);
            _mockStateObserver.Verify(o => o.OnCompleted());
            _mockMessageObserver.Verify(o => o.OnCompleted());

        }

        [Test]
        public async Task OnNewMessage()
        {
            var testMessage = "test";
            await _realTimeService.OnNewMessage(testMessage);
            
            _mockMessageObserver.Verify(o => o.OnNext(testMessage));
        }

        [Test]
        public async Task ConnectionLost_Notifies()
        {
            await _realTimeService.Connect("test");

            _mockHubProxy.SetupGet(m => m.State)
                .Returns(HubConnectionState.Disconnected);

            _mockHubProxy.Raise(m => m.Closed += null, default(object));
            _mockStateObserver.Verify(m => m.OnNext(HubConnectionState.Disconnected));


            _mockHubProxy.SetupGet(m => m.State)
                .Returns(HubConnectionState.Reconnecting);

            _mockHubProxy.Raise(m => m.Reconnecting += null, default(object));
            _mockStateObserver.Verify(m => m.OnNext(HubConnectionState.Reconnecting));
        }

        [Test]
        public async Task ConnectionLost_WithException_NotifiesError()
        {
            await _realTimeService.Connect("test");

            var erromessage = "test exception";

            _mockHubProxy.Raise(m => m.Closed += null, new Exception(erromessage));
            _mockStateObserver.Verify(m => m.OnError(It.Is<Exception>(e => e.Message == erromessage)));

            _mockHubProxy.Raise(m => m.Reconnecting += null, new Exception(erromessage));
            _mockStateObserver.Verify(m => m.OnError(It.Is<Exception>(e => e.Message == erromessage)));
        }

        [Test]
        public async Task Reconnected_Notifies()
        {
            await _realTimeService.Connect("test");

            _mockHubProxy.SetupGet(m => m.State)
                .Returns(HubConnectionState.Connected);

            _mockHubProxy.Raise(m => m.Reconnected += null, default(object));
            _mockStateObserver.Verify(m => m.OnNext(HubConnectionState.Connected));
        }
    }
}
