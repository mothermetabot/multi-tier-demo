using System;
using System.Windows;
using Application1.Services;

namespace Application1.ConnectionWindow
{
    public partial class ConnectionView : Window, IViewGeneric<ConnectionViewModel>
    {
        public ConnectionView(ConnectionViewModel viewModel)
        {
            DataContext = ViewModel = viewModel 
                ?? throw new ArgumentNullException(nameof(viewModel));

            InitializeComponent();
            Closing += (s, e) => Shutdown?.Invoke(this);
        }

        public ConnectionViewModel ViewModel { get; }

        public event Action<IView>? Shutdown;
    }
}
