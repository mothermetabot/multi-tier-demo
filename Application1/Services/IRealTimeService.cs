using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Application1.Services
{
    public interface IRealTimeService : IAsyncDisposable
    {
        string Name { get; set; }

        IObservable<string> MessageObservable { get; }

        IObservable<HubConnectionState> ConnectionStateObservable { get; }

        Task Connect(string name);

        Task Broadcast(string content);
    }
}
