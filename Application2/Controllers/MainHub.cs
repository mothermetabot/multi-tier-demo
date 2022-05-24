using Common.DataTransferObject;
using Microsoft.AspNetCore.SignalR;
using Application2.Extensions;
using Application2.Models;
using Application2.Services;
using Common.Structs;

namespace Application2.Controllers
{
    public class MainHub : Hub
    {
        public MainHub(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor,
            IMessageValidator validator)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _validator = validator;
        }


        private readonly IUserRepository _userRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IMessageValidator _validator;

        public override async Task OnConnectedAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext
                ?? throw new ArgumentException("Couldn't get http context for current user.");

            if (!httpContext.Request.Query
                .TryGetValue(QueryKey.NAME, out var usernameValues))
                throw new ArgumentException("username", "User has not provided an username.");

            var username = usernameValues.First();

            var user = new User(Context.ConnectionId, username);
            _userRepository.Add(user);

            await this.NotifiyUserConnected(user);
            
           await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = _userRepository.Delete(Context.ConnectionId);
            if (user == null) return;

            await this.NotifiyUserDisconnected(user);

            await base.OnDisconnectedAsync(exception);
        }


        [HubMethodName(RemoteProcedureCall.SEND_MESSAGE)]
        public async Task SendMessage(UserMessageDto userMessage)
        {
            if (userMessage == null || userMessage.UserId == null)
                    return;

            userMessage.Content ??= string.Empty;

            var sanitized = _validator.Sanitize(userMessage.Content);

            await Clients.Client(userMessage.UserId)
                .SendAsync(RemoteProcedureCall.RECEIVE_MESSAGE,
                sanitized);
        }


        [HubMethodName(RemoteProcedureCall.SEND_BROADCAST)]
        public async Task BroadcastMessage(string message)
        {
            if (message == null)
                return;

            var sanitized = _validator.Sanitize(message);

            var userMessage = new UserMessageDto {
                Content = sanitized,
                UserId = Context.ConnectionId
            };

            await Clients.All.SendAsync(RemoteProcedureCall.RECEIVE_BROADCAST,
                userMessage);
        }


        [HubMethodName(RemoteProcedureCall.GET_USERS)]
        public Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = _userRepository.Get()
                .Select(u => u.ToDto());

            return Task.FromResult(users);
        }
    }
}
