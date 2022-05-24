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
    internal class ConnectionViewModelTests
    {
        private Mock<INavigator> _mockNavigator;

        private Mock<IRealTimeService> _mockRtService;

        private ConnectionViewModel _viewModel;

        private Subject<HubConnectionState> _hubConnectionStateSubject;

        private Mock<IViewGeneric<ConnectionViewModel>> _mockConnectionView;

        [SetUp]
        public void Setup()
        {
            _mockNavigator = new();
            _mockRtService = new();
            _mockConnectionView = new();
            _hubConnectionStateSubject = new Subject<HubConnectionState>();

            _mockNavigator.SetupGet(m => m.CurrentView)
                .Returns(_mockConnectionView.Object);

            _mockRtService.SetupGet(m => m.ConnectionStateObservable)
                .Returns(_hubConnectionStateSubject);

            _viewModel = new ConnectionViewModel(_mockNavigator.Object,
                _mockRtService.Object);
        }

        [Test]
        public async Task Connected_NavigatesToMainView()
        {
            _mockRtService.Setup(c => c.Connect(It.IsAny<string>()))
             .Callback(() => _hubConnectionStateSubject.OnNext(HubConnectionState.Connected));

            _viewModel.Name = "name";
            await _viewModel.OnConnect();
            _mockNavigator.Verify(n => n.NavigateTo<MainViewModel>(), Times.Once);
        }

        [Test]
        public async Task ConnectCommand_IsEnabledFlagSet()
        {
            _mockRtService.Setup(c => c.Connect("name"))
                .Callback(() => _hubConnectionStateSubject.OnNext(HubConnectionState.Connected));
            _viewModel.Name = "name";
            await _viewModel.OnConnect();

            Assert.IsFalse(_viewModel.IsEnabled);
        }

        [Test]
        public void ConnectCommand_NameNullOrWhiteSpace()
        {
            // name null
            Assert.IsFalse(_viewModel.IsEnabled);
            _viewModel.Name = null;
            _viewModel.ConnectCommand.Execute(null);

            _mockNavigator.Verify(m => m.MessageBox(It.IsAny<string>()));
            Assert.IsTrue(_viewModel.IsEnabled);

            _mockNavigator.VerifyNoOtherCalls();
            Setup();

            // name empty
            Assert.IsFalse(_viewModel.IsEnabled);
            _viewModel.Name = string.Empty;
            _viewModel.ConnectCommand.Execute(null);

            _mockNavigator.Verify(m => m.MessageBox(It.IsAny<string>()));
            Assert.IsTrue(_viewModel.IsEnabled);

            _mockNavigator.VerifyNoOtherCalls();
            Setup();

            // name white space
            Assert.IsFalse(_viewModel.IsEnabled);
            _viewModel.Name = "   ";
            _viewModel.ConnectCommand.Execute(null);

            _mockNavigator.Verify(m => m.MessageBox(It.IsAny<string>()));
            Assert.IsTrue(_viewModel.IsEnabled);

            _mockNavigator.VerifyNoOtherCalls();
        }

        [Test]
        public async Task ConnectCommand_NotConnected()
        {
            Assert.IsFalse(_viewModel.IsEnabled);

            _mockRtService.Setup(c => c.Connect(It.IsAny<string>()))
                .Callback(() => {
                    _hubConnectionStateSubject.OnNext(HubConnectionState.Disconnected);
                });

            await _viewModel.OnConnect();

            Assert.IsTrue(_viewModel.IsEnabled);
            _mockNavigator.Verify(n => n.MessageBox(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ConnectCommand_OnError()
        {
            Assert.IsFalse(_viewModel.IsEnabled);

            _mockRtService.Setup(c => c.Connect(It.IsAny<string>()))
                .Callback(() => _hubConnectionStateSubject.OnError(new System.Exception("Error")));

            _viewModel.Name = "name";
            await _viewModel.OnConnect();

            Assert.IsTrue(_viewModel.IsEnabled);
            _mockNavigator.Verify(n => n.MessageBox(It.IsAny<string>()), Times.Once);
        }

    }
}
