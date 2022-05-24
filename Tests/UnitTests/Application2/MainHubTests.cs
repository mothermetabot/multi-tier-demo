using Common.DataTransferObject;
using Common.Enumerations;
using Common.Structs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application2.Controllers;
using Application2.Models;
using Application2.Services;

namespace Tests.UnitTests.Application2
{
    public class MainHubTests
    {
        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new();
            _mockValidator = new();
            _mockValidator.Setup(v => v.Validate(It.IsAny<string>()))
                .Returns(true);

            _mockValidator.Setup(v => v.Sanitize(It.IsAny<string>()))
                .Returns<string>(str => str);
            SetupHubContext();

            SetupHubClientProxies();


            _mainHub = new MainHub(_mockUserRepository, _mockHttpContextAccessor.Object, _mockValidator.Object);
            _mainHub.Context = _mockHubCallerContext.Object;
            _mainHub.Clients = _mockClients.Object;
        }

        private void SetupHubClientProxies()
        {
            _mockClients = new();
            _mockClientProxy = new();

            // setup client proxies

            _mockClients.SetupGet(clients => clients.All)
                .Returns(_mockClientProxy.Object);

            _mockClients.Setup(clients => clients.Client(It.IsAny<string>()))
                .Returns(_mockClientProxy.Object);
        }

        private void SetupHubContext()
        {
            _httpContext = new();
            _mockHttpContextAccessor = new();
            _mockHubCallerContext = new();

            _mockHttpContextAccessor.SetupGet(accessor => accessor.HttpContext)
               .Returns(_httpContext.Object);

            _mockHubCallerContext.SetupGet(context => context.ConnectionId)
                .Returns("TEST-CONNECTIONID");

            // setup caller context object
            var querycollection = new QueryCollection(new Dictionary<string, StringValues> {
                {QueryKey.NAME, "name" }
            });

            _httpContext.SetupGet(http => http.Request.Query)
                .Returns(querycollection);
        }


#pragma warning disable CS8618 // Value is always set in Setup method
        private Mock<IClientProxy> _mockClientProxy;

        private Mock<IHubCallerClients> _mockClients;

        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        private Mock<HubCallerContext> _mockHubCallerContext;

        private MainHub _mainHub;

        private Mock<HttpContext> _httpContext;

        private MockUserRepository _mockUserRepository;

        private Mock<IMessageValidator> _mockValidator;
#pragma warning restore CS8618

        #region ON CONNECTED
        [Test]
        public async Task OnConnected_AddsUserToRepository()
        {
            await _mainHub.OnConnectedAsync();

            var users = _mockUserRepository.Get();

            CollectionAssert.IsNotEmpty(users);
            CollectionAssert.AllItemsAreNotNull(users);
        }

        [Test]
        public async Task OnConnected_NotifiesConnection()
        {
            await _mainHub.OnConnectedAsync();

            _mockClientProxy.Verify(proxy =>
                proxy.SendCoreAsync(RemoteProcedureCall.RECEIVE_USER_ACTIVITY,
                    It.Is<object[]>(obj => obj[0].GetType() == typeof(UserActivityDto)
                        && ((UserActivityDto)obj[0]).Type == UserActivityType.Connection),
                    CancellationToken.None));

            Assert.Pass();
        }

        [Test]
        public void OnConnected_NoNameInQuery_ThrowsException()
        {
            // override valid query mock to invalid
            var querycollection = new QueryCollection();

            _httpContext.SetupGet(http => http.Request.Query)
                .Returns(querycollection);

            Assert.ThrowsAsync<ArgumentException>(_mainHub.OnConnectedAsync);
        }
        #endregion

        #region ON DISCONNECTED
        [Test]
        public async Task OnDisconnected_RemovesUserFromRepository()
        {
            var user = new User("TEST-CONNECTIONID", "NAME");
            _mockUserRepository.Add(user);

            var users = _mockUserRepository.Get();
            var getUser = _mockUserRepository.Get(user.Id);

            Assert.IsNotEmpty(users);
            Assert.AreSame(user, getUser);

            await _mainHub.OnDisconnectedAsync(null);

            users = _mockUserRepository.Get();
            Assert.IsEmpty(users);
        }
        [Test]
        public async Task OnDisconnected_NotifiesDisconnection()
        {
            var user = new User("TEST-CONNECTIONID", "NAME");
            _mockUserRepository.Add(user);

            await _mainHub.OnDisconnectedAsync(null);

            _mockClientProxy.Verify(proxy =>
                proxy.SendCoreAsync(RemoteProcedureCall.RECEIVE_USER_ACTIVITY,
                    It.Is<object[]>(obj => obj[0].GetType() == typeof(UserActivityDto)
                        && ((UserActivityDto)obj[0]).Type == UserActivityType.Disconnection),
                    CancellationToken.None));

            Assert.Pass();
        }
        #endregion

        #region GET USERS
        [Test]
        public async Task GetUsers_ReturnsUsers()
        {
            var user = new User("TEST-CONNECTIONID", "NAME");
            _mockUserRepository.Add(user);

            var users = await _mainHub.GetUsers();

            CollectionAssert.IsNotEmpty(users);

            foreach (var item in users) {
                if (item.Id == user.Id) Assert.Pass();
            }
        }
        #endregion

        #region SEND MESSAGE
        [Test]
        public async Task SendMessage_SendsMessage()
        {
            var usermessage = new UserMessageDto() {
                Content = "content",
                UserId = "userId"
            };

            await _mainHub.SendMessage(usermessage);

            _mockClientProxy.Verify(proxy =>
                proxy.SendCoreAsync(RemoteProcedureCall.RECEIVE_MESSAGE,
                    It.Is<object[]>(obj => obj[0].GetType() == typeof(string)),
                    CancellationToken.None));
        }
        #endregion

        #region SEND BROADCAST
        [Test]
        public async Task SendBroadcast_SendsBroadcast()
        {
            var broadcast = "hallo";

            await _mainHub.BroadcastMessage(broadcast);

            _mockClientProxy.Verify(proxy =>
                proxy.SendCoreAsync(RemoteProcedureCall.RECEIVE_BROADCAST,
                    It.Is<object[]>(obj => obj[0].GetType() == typeof(UserMessageDto)
                        && ((UserMessageDto)obj[0]).Content == broadcast
                        && ((UserMessageDto)obj[0]).UserId == _mockHubCallerContext.Object.ConnectionId),
                    CancellationToken.None));
        }
        #endregion
    }
}
