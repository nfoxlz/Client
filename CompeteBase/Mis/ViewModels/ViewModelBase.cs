using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Compete.Mis.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        /// <summary>
        /// 通知属性改变方法。
        /// </summary>
        /// <param name="propertyName">属性名。</param>
        public void NotifyPropertyChanged([CallerMemberName] string? propertyName = null) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
}
