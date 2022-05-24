namespace Application1.Services
{
    public interface IViewGeneric<TviewModel> : IView where TviewModel : ViewModelBase
    {
        TviewModel ViewModel { get; }
    }
}
