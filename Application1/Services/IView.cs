using System;

namespace Application1.Services
{
    public interface IView
    {
        string Title { get; }

        void Hide();

        void Show();

        event Action<IView>? Shutdown;
    }
}
