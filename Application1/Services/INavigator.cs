using System;

namespace Application1.Services
{
    public interface INavigator
    {
        public IView? CurrentView { get; }

        void MessageBox(string message);

        void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : ViewModelBase;

        void NavigateTo(Type t, object? parameter = null);
    }
}
