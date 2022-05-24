using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application1.Services
{
    /// <summary>
    /// A mockable wrapper for the <see cref="Microsoft.AspNetCore.SignalR.Client.HubConnection"/> class.
    /// </summary>
    public interface IHubProxy : IAsyncDisposable
    {

        event Func<Exception?, Task>? Closed;

        event Func<Exception?, Task>? Reconnecting;

        event Func<string?, Task>? Reconnected;

        HubConnectionState State { get; }

        Task SendAsync(string methodName,
            object? arg1,
            CancellationToken cancellationToken = default(CancellationToken));

        Task StartAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task StopAsync(CancellationToken cancellationToken = default(CancellationToken));

        IDisposable On<T1>(string methodName, Func<T1, Task> handler);

        Task BuildAsync(string url);
    }
}
