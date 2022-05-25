using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Windows;
using Application1.ConnectionWindow;
using Application1.Services;

namespace Application1.MainWindow
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(INavigator navigator, IRealTimeService realTimeService)
        {
            _realTimeService = realTimeService;
            _navigator = navigator;

            _realTimeService.ConnectionStateObservable
                .Subscribe(HandleNextState, HandleErrorState);

            _realTimeService.MessageObservable
                .Subscribe(HandleNextMessage);
        }


        private readonly IRealTimeService _realTimeService;

        private readonly INavigator _navigator;


        public string? ReceivedText {
            get => _recveivedText;
            set => Set(nameof(ReceivedText), ref _recveivedText, value);
        }
        private string? _recveivedText;

        public IObservable<string>? SentTextObservable {
            get => _sentTextObservable;
            set {
                _sentTextObservable = value;
                _sentTextObservable?.Subscribe(NotifyAndForget);
            }
        }

        private IObservable<string>? _sentTextObservable;

        public bool IsConnected {
            get => _isConnected;
            set => Set(nameof(IsConnected), ref _isConnected, value);
        }
        private bool _isConnected = false;

        public string? Name {
            get => _name;
            set => Set(nameof(Name), ref _name, value);
        }
        private string? _name;

        public async void NotifyAndForget(string content)
        {
            try {
                await _realTimeService.Broadcast(content);

            } catch (Exception e) {
                Debug.WriteLine(e);
            }
        }

        private void HandleNextMessage(string content)
        {
            ReceivedText = content;
        }

        private void HandleErrorState(Exception error)
        {
            IsConnected = false;
            _navigator.MessageBox("Error occurred: " + error);

            _navigator.NavigateTo<ConnectionViewModel>();
        }

        private void HandleNextState(HubConnectionState state)
        {
            IsConnected = false;

            switch (state) {

                case HubConnectionState.Disconnected:

                    _navigator.MessageBox("Disconnected from service.");
                    _navigator.NavigateTo<ConnectionViewModel>();

                    break;

                case HubConnectionState.Connected:
                    IsConnected = true;

                    break;

                case HubConnectionState.Reconnecting:
                    _navigator.MessageBox("Connection lost. Attempting reconnection.");
                    break;
            }
        }
    }
}
