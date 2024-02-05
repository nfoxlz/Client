using CommunityToolkit.Mvvm.ComponentModel;

namespace Compete.Mis.Frame.ViewModels
{
    internal abstract class PageViewModel : ObservableObject
    {
        public abstract void Refresh();
    }
}
