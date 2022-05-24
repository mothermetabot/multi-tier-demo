using System.Windows;
using Application1.ConnectionWindow;
using Application1.MainWindow;
using Application1.Services;
using Application1.Structs;

namespace Application1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SingletonService.Clear();

            var hubProxy = new HubProxy();
            var realTimeService = new RealTimeService(hubProxy);
            SingletonService.Register<IRealTimeService, RealTimeService>(realTimeService);

            var navigator = new DefaultNavigator(Dispatcher, (d, v) => d.InvokeShutdown());
            SingletonService.Register<INavigator, DefaultNavigator>(navigator);

            var connectionViewModel = new ConnectionViewModel(navigator,
                realTimeService);
            var connectionView = new ConnectionView(connectionViewModel);

            var mainViewModel = new MainViewModel(navigator,
                realTimeService);

            var mainView = new MainView(mainViewModel);

            navigator.RegisterView(connectionView);
            navigator.RegisterView(mainView);

            navigator.NavigateTo<ConnectionViewModel>();
        }
    }
}
