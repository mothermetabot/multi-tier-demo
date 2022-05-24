using Common.Structs;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Application1.Services
{
    public class RealTimeService : IRealTimeService
    {
        public RealTimeService(IHubProxy proxy)
        {
            _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
        }

        public IObservable<HubConnectionState> ConnectionStateObservable => _connectionStateSubject;

        private readonly ISubject<HubConnectionState> _connectionStateSubject = new Subject<HubConnectionState>();

        public IObservable<string> MessageObservable => _messageSubject;

        private readonly ISubject<string> _messageSubject = new Subject<string>();

        private readonly IHubProxy _proxy;

        public async Task Broadcast(string content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            await _proxy.SendAsync(RemoteProcedureCall.SEND_BROADCAST, content);
        }

        public async Task Connect(string url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException(nameof(url));

            await _proxy.BuildAsync(url);

            _proxy.Closed += OnClosed;
            _proxy.Reconnected += NotifyConnectionState;
            _proxy.Reconnecting += OnClosed;

            _proxy.On<string>(RemoteProcedureCall.RECEIVE_MESSAGE, OnNewMessage);

            await _proxy.StartAsync()
                .ContinueWith(t => NotifyConnectionState(null));
        }

        private Task NotifyConnectionState(string? arg)
        {
            _connectionStateSubject.OnNext(_proxy.State);

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await _proxy.StopAsync();
            await _proxy.DisposeAsync();

            _connectionStateSubject.OnCompleted();
            _messageSubject.OnCompleted();
        }


        public Task OnNewMessage(string content)
        {
            _messageSubject.OnNext(content);

            return Task.CompletedTask;
        }

        private Task OnClosed(Exception? arg)
        {
            if (_proxy == null) throw new NullReferenceException("The connection object is null");

            if (arg != null)
                _connectionStateSubject.OnError(arg);

            else
                _connectionStateSubject.OnNext(_proxy.State);

            return Task.CompletedTask;
        }
    }
}
