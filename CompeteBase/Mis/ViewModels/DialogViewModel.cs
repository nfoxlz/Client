// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/12 14:43:34 LeeZheng 新建。
//==============================================================
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Compete.Mis.ViewModels
{
    /// <summary>
    /// DialogViewModel 类。
    /// </summary>
    public abstract partial class DialogViewModel : ViewModelBase
    {
        [RelayCommand(CanExecute = nameof(CanOk))]
        private void Ok(FrameworkElement sender) => DoOk(sender);

        protected virtual void DoOk(FrameworkElement sender) => sender.GetWindow()!.DialogResult = true;

        protected abstract bool CanOk();
    }
}
