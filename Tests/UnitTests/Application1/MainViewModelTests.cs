using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using NUnit.Framework;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Application1.ConnectionWindow;
using Application1.MainWindow;
using Application1.Services;

namespace Tests.UnitTests.Application1
{
    public class MainViewModelTests
    {
        private Mock<INavigator> _mockNavigator;

        private Mock<IRealTimeService> _mockRtService;

        private MainViewModel _viewModel;

        private Subject<HubConnectionState> _hubConnectionStateSubject ;
        private Subject<string> _hubConnectionMessageSubject;

        private Mock<IViewGeneric<MainViewModel>> _mockConnectionView;

        [SetUp]
        public void Setup()
        {
            _mockNavigator = new();
            _mockRtService = new();
            _mockConnectionView = new();
            _hubConnectionStateSubject = new Subject<HubConnectionState>();
            _hubConnectionMessageSubject = new Subject<string>();

            _mockNavigator.SetupGet(m => m.CurrentView)
                .Returns(_mockConnectionView.Object);

            _mockRtService.SetupGet(m => m.ConnectionStateObservable)
                .Returns(_hubConnectionStateSubject);


            _mockRtService.SetupGet(m => m.MessageObservable)
                .Returns(_hubConnectionMessageSubject);

            _viewModel = new MainViewModel(_mockNavigator.Object,
                _mockRtService.Object);
        }


        [Test]
        public void OnConnectionLost()
        {
            _viewModel.IsConnected = true;


            // navigates back to connection view
            _hubConnectionStateSubject.OnNext(HubConnectionState.Disconnected);
            _mockNavigator.Verify(m => m.NavigateTo<ConnectionViewModel>());
            Assert.IsFalse(_viewModel.IsConnected);


            // displays message box and navigates back
            _hubConnectionStateSubject.OnError(new System.Exception());
            _mockNavigator.Verify(m => m.NavigateTo<ConnectionViewModel>());

            _mockNavigator.Verify(m => m.MessageBox(It.IsAny<string>()));

            // flag is set to false
            Assert.IsFalse(_viewModel.IsConnected);
        }

        [Test]
        public async Task OnReconnecting()
        {
            _viewModel.IsConnected = true;
            await Task.Delay(100);

            _hubConnectionStateSubject.OnNext(HubConnectionState.Reconnecting);

            // displays messagebox
            _mockNavigator.Verify(m => m.MessageBox(It.IsAny<string>()));


            // sets flag to false
            Assert.IsFalse(_viewModel.IsConnected);
        }

        [Test]
        public void Connected_SetsIsConnectedFlag()
        {
            _hubConnectionStateSubject.OnNext(HubConnectionState.Connected);
            Assert.IsTrue(_viewModel.IsConnected);
        }

        [Test]
        public void MessageReceived_UpdatesReceivedText()
        {
            var msg = "Message";
            _hubConnectionMessageSubject.OnNext(msg);

            Assert.IsTrue(_viewModel.ReceivedText == msg);
        }

        [Test]
        public async Task SendTextObservable_BroadcastsMessage()
        {
            var msgObs = new Subject<string>();

            _viewModel.SentTextObservable = msgObs;
            msgObs.OnNext("Sent text");

            await Task.Delay(100);

            _mockRtService.Verify(r => r.Broadcast(It.IsAny<string>()));
        }
    }
}
