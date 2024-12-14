using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Compete.Mis.Frame.ViewModels
{
    internal sealed partial class ModifyPasswordViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ModifyCommand))]
        private string _originalPassword = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ModifyCommand))]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ModifyCommand))]
        private string _verifiedPassword = string.Empty;

        [RelayCommand(CanExecute = nameof(CanModify))]
        private void Modify(Window dialog)
        {
            if (Services.GlobalServices.FrameService.ModifyPassword(OriginalPassword, NewPassword.Trim()))
            {
                VerifiedPassword = NewPassword = OriginalPassword = string.Empty;
                MisControls.MessageDialog.Success("ModifyPasswordBox.Success");
                dialog.DialogResult = true;
            }
            else
                MisControls.MessageDialog.Warning("ModifyPasswordBox.Failed");
        }

        private bool CanModify() => !string.IsNullOrWhiteSpace(OriginalPassword) && !string.IsNullOrWhiteSpace(NewPassword) && NewPassword.Trim().Length >= 6 && NewPassword == VerifiedPassword;

    }
}
