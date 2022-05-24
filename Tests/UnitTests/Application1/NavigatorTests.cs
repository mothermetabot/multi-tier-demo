using Moq;
using NUnit.Framework;
using System;
using System.Windows.Threading;
using Application1;
using Application1.Services;

namespace Tests.UnitTests.Application1
{
    public class NavigatorTests
    {
        private DefaultNavigator _navigator;


        public Mock<ViewModelBase> _mockVm;

        private Mock<IViewGeneric<ViewModelBase>> _mockView;

        private Mock<IDisposable> _mockCloseDisposable;


        [SetUp]
        public void Setup()
        {
            _mockCloseDisposable = new();

            _navigator = new(Dispatcher.CurrentDispatcher, 
                (m,v) => _mockCloseDisposable.Object.Dispose());

            _mockVm = new();
            _mockView = new();
        }


        [Test]
        public void RegisterView()
        {
            _navigator.RegisterView(_mockView.Object);

            CollectionAssert.Contains(_navigator.Map.Values, _mockView.Object);

            IViewGeneric<ViewModelBase> @null = null;
            Assert.Throws<ArgumentNullException>(() => _navigator.RegisterView(@null));
            Assert.Throws<ArgumentException>(() => _navigator.RegisterView(_mockView.Object));
        }

        [Test]
        public void NavigateTo()
        {
            _navigator.RegisterView(_mockView.Object);

            _navigator.NavigateTo<ViewModelBase>();
            _navigator.NavigateTo<ViewModelBase>();

            _mockView.Verify(view => view.Show(), Times.Exactly(2));
            _mockView.Verify(view => view.Hide(), Times.Once);
        }

        [Test]
        public void ViewClose_TriggersOnClose()
        {
            _navigator.RegisterView(_mockView.Object);
            _navigator.NavigateTo<ViewModelBase>();

            _mockView.Raise(v => v.Shutdown += null, _mockView.Object);
            _mockCloseDisposable.Verify(d => d.Dispose(), Times.Once);
        }
    }
}
