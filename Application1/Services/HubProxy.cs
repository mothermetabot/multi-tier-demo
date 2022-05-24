using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application1.Services
{
    public class HubProxy : IHubProxy
    {
#pragma warning disable CS8602 // Null reference is checked in ThrowForNotBuilt method.
        private HubConnection? _connection;

        public HubConnectionState State => _connection?.State ?? HubConnectionState.Disconnected;

        public event Func<Exception?, Task>? Closed;

        public event Func<Exception?, Task>? Reconnecting;
         
        public event Func<string?, Task>? Reconnected;

        public Task BuildAsync(string url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException(nameof(url));

            var connection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();

            _connection = connection;

            _connection.Closed += Closed;
            _connection.Reconnected += Reconnected;
            _connection.Reconnecting += Reconnecting;

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            ThrowForNotBuilt();
            await  _connection.DisposeAsync();
            _connection = null;
        }

        public IDisposable On<T1>(string methodName, Func<T1, Task> handler)
        {
            ThrowForNotBuilt();
            return _connection.On(methodName, handler);
        }

        public Task SendAsync(string methodName, object? arg1, CancellationToken cancellationToken = default)
        {
            ThrowForNotBuilt();
            return _connection.SendAsync(methodName, arg1, cancellationToken);
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            ThrowForNotBuilt();
            return _connection.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            ThrowForNotBuilt();
            return _connection.StopAsync(cancellationToken);
        }

        private void ThrowForNotBuilt()
        {
            if (_connection == null) throw new InvalidOperationException("Hub object not built.");
        }
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
    }
}
