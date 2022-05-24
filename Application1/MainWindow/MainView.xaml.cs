using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Application1.Services;

namespace Application1.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window, IViewGeneric<MainViewModel>
    {
        public MainView(MainViewModel vm)
        {
            DataContext = ViewModel = vm ?? throw new ArgumentNullException(nameof(vm));
            InitializeComponent();
            
            var textObservableSubscription = Observable
                .FromEventPattern<TextChangedEventArgs>(_sendBox, nameof(_sendBox.TextChanged))
                .Select(it => _sendBox.Text)
                .Throttle(TimeSpan.FromMilliseconds(20));

            ViewModel.SentTextObservable = textObservableSubscription;

            Closing += (s, e) => Shutdown?.Invoke(this);
        }

        public MainViewModel ViewModel { get; }

        public event Action<IView>? Shutdown;
    }
}
