using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UITemplate
{
    internal sealed partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ushort _pageSize;

        [ObservableProperty]
        private ulong _currentPageNo;

        [ObservableProperty]
        private ulong _recordCount;

        [RelayCommand]
        private void Test()
        {
        }

        [RelayCommand]
        private void Test1()
        {
            RecordCount = 100UL;
            //OnPropertyChanged(nameof(RecordCount));
        }
    }
}
