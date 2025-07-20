using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compete.Mis.Chart
{
    internal sealed partial class ChartSettingViewModel : ViewModels.DialogViewModel
    {
        [ObservableProperty]
        private ChartSetting? _chartSetting;

        protected override bool CanOk() => true;
    }
}
