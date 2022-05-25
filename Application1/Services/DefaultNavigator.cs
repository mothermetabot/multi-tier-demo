using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Application1.Services
{
    public class DefaultNavigator : INavigator
    {
        public DefaultNavigator(Dispatcher dispatcher, Action<Dispatcher, IView> onClose)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            _onClose = onClose ?? throw new ArgumentNullException(nameof(onClose));
        }
        /// <summary>
        /// For testing purposes only.
        /// </summary>
        public Dictionary<Type, IView> Map => _map;

        private readonly Dictionary<Type, IView> _map = new Dictionary<Type, IView>();

        private readonly Dispatcher _dispatcher;

        private readonly Action<Dispatcher, IView> _onClose;


        public IView? CurrentView { get; private set; }

        public void NavigateTo<Tto>(object? parameter = null) where Tto : ViewModelBase
        {
            NavigateTo(typeof(Tto), parameter);
        }

        public void NavigateTo(Type t, object? parameter = null)
        {
            if (!t.IsAssignableTo(typeof(ViewModelBase)))
                throw new InvalidOperationException("The specified type is not of " + typeof(ViewModelBase));

            var toView = _map[t];

            _dispatcher.Invoke(() => {

                toView.Show(parameter);
                CurrentView?.Hide();

                CurrentView = toView;
            }, DispatcherPriority.ContextIdle);
        }

        public void RegisterView<TViewModel>(IViewGeneric<TViewModel> view) where TViewModel : ViewModelBase
        {
            if (view is null) throw new ArgumentNullException(nameof(view));


            view.Shutdown += (inView) => {
                _onClose(_dispatcher, inView);
            };

            _map.Add(typeof(TViewModel), view);
        }

        public void MessageBox(string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException(nameof(message));

            _dispatcher.InvokeAsync(() =>
                System.Windows.MessageBox.Show(message));
        }
    }
}
