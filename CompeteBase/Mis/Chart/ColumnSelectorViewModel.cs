using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Windows;

namespace Compete.Mis.Chart
{
    internal sealed partial class ColumnSelectorViewModel : ViewModels.DialogViewModel
    {
        [ObservableProperty]
        private IDictionary<string, string>? _columnNames;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private KeyValuePair<string, string>? _selectedItem;

        protected override bool CanOk() => null != SelectedItem;
    }
}
