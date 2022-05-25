using Common.Structs;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Input;
using Application1.MainWindow;
using Application1.Services;
using Application1.Structs;

namespace Application1.ConnectionWindow
{
    public class ConnectionViewModel : ViewModelBase
    {
        public ConnectionViewModel(INavigator navigator, IRealTimeService realTimeService)
        {
            _navigator = navigator;
            _realTimeService = realTimeService ??
                throw new ArgumentNullException(nameof(realTimeService));

            ConnectCommand = new ParameterlessCommand(async () => await OnConnect());
        }

        private readonly INavigator _navigator;
        private readonly IRealTimeService _realTimeService;

        public string Name {
            get => _name;
            set => Set(nameof(Name), ref _name, value);
        }
        private string _name = string.Empty;

        public bool IsEnabled {
            get => _isEnabled;
            set => Set(nameof(IsEnabled), ref _isEnabled, value);
        }
        private bool _isEnabled;

        public ICommand ConnectCommand { get; }

        private IDisposable? _connectionDisposable;


        public Task OnConnect()
        {
            IsEnabled = false;

            if (string.IsNullOrWhiteSpace(Name)) {
                _navigator.MessageBox("Please set a name that is not empty or white space.");
                IsEnabled = true;
                return Task.CompletedTask;
            }

            _connectionDisposable = _realTimeService.ConnectionStateObservable
                .Subscribe(onNext: (state) => {

                    _connectionDisposable?.Dispose();

                    if (state == HubConnectionState.Connected) {
                        _navigator.NavigateTo<MainViewModel>(_realTimeService.Name);
                        return;
                    }

                    _navigator.MessageBox("Couldn't connect to real time service.");
                    IsEnabled = true;

                },
                onError: (e) => {
                    _navigator.MessageBox("Error occurred while trying to connect.\r\n " + e);
                    _connectionDisposable?.Dispose();
                    IsEnabled = true;
                });

            _realTimeService.Name = Name;

            var url = UrlHelper.BuildUrl(Constant.REAL_TIME_SERVICE_URI, Name);
            return _realTimeService.Connect(url);
        }
    }
}
