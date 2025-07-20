using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Compete.Extensions;
using ScottPlot.WPF;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;

namespace Compete.Mis.Chart
{
    internal sealed partial class ChartViewModel : ViewModels.ViewModelBase
    {
        private IDictionary<string, string> columnNames = new OrderedDictionary<string, string>();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveChartSettingCommand))]
        [NotifyCanExecuteChangedFor(nameof(ModifyChartSettingCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddColumnNameCommand))]
        private ChartSetting? _selectedChartSetting;

        [ObservableProperty]
        private IEnumerable? _masterData;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private ICollection<KeyValuePair<string, string>>? _currentColumnNames;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveColumnNameCommand))]
        private KeyValuePair<string, string>? _currentColumnName;

        [ObservableProperty]
        private GridLength _settingWidth = new GridLength(0.25, GridUnitType.Star);

        [ObservableProperty]
        private string? _xLabel;

        [ObservableProperty]
        private string? _yLabel;

        public WpfPlot PlotControl { get; } = new WpfPlot();

        public IList<ChartSetting> ChartSettings { get; } = new ObservableCollection<ChartSetting>();

        public ChartViewModel() => PlotControl.Plot.Font.Set(SKFontManager.Default.MatchCharacter('汉').FamilyName);

        [RelayCommand]
        private void AddChartSetting()
        {
            var dialog = new ChartSettingDialog();
            var vm = new ChartSettingViewModel();
            vm.ChartSetting = new ChartSetting();
            dialog.DataContext = vm;
            if (dialog.ShowDialog() != true)
                return;
            ChartSettings.Add(vm.ChartSetting);
            RefreshCommand.NotifyCanExecuteChanged();
        }

        private bool CanOperateChartSetting() => SelectedChartSetting is not null && ChartSettings.Contains(SelectedChartSetting);


        [RelayCommand(CanExecute = nameof(CanOperateChartSetting))]
        private void ModifyChartSetting()
        {
            var dialog = new ChartSettingDialog();
            var vm = new ChartSettingViewModel();
            vm.ChartSetting = new ChartSetting();
            vm.ChartSetting.TypeSetting = SelectedChartSetting!.TypeSetting;
            vm.ChartSetting.FillColor = SelectedChartSetting.FillColor;
            vm.ChartSetting.LineColor = SelectedChartSetting.LineColor;
            vm.ChartSetting.ColumnNames.Clear();
            foreach (var kv in SelectedChartSetting.ColumnNames)
                vm.ChartSetting.ColumnNames.Add(kv.Key, kv.Value);
            dialog.DataContext = vm;
            if (dialog.ShowDialog() != true)
                return;

            var index = ChartSettings.IndexOf(SelectedChartSetting);
            ChartSettings.Remove(SelectedChartSetting);
            ChartSettings.Insert(index, vm.ChartSetting);
            SelectedChartSetting = vm.ChartSetting;
        }

        [RelayCommand(CanExecute = nameof(CanOperateChartSetting))]
        private void RemoveChartSetting()
        {
            ChartSettings.Remove(SelectedChartSetting!);
            RefreshCommand.NotifyCanExecuteChanged();
        }

        private bool CanAddColumnName() => CurrentColumnNames is not null && CanOperateChartSetting();

        [RelayCommand(CanExecute = nameof(CanAddColumnName))]
        private void AddColumnName()
        {
            var dialog = new ColumnSelectorDialog();
            var vm = (dialog.DataContext as ColumnSelectorViewModel)!;
            vm.ColumnNames = columnNames;
            if (dialog.ShowDialog() == true && !SelectedChartSetting!.ColumnNames.ContainsKey(vm.SelectedItem!.Value.Key))
            {
                SelectedChartSetting!.ColumnNames.Add(vm.SelectedItem!.Value.Key, vm.SelectedItem.Value.Value);
                CurrentColumnNames!.Add(new KeyValuePair<string, string>(vm.SelectedItem!.Value.Key, vm.SelectedItem.Value.Value));
                RefreshCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand(CanExecute = nameof(CanRemoveColumnName))]
        private void RemoveColumnName()
        {
            SelectedChartSetting!.ColumnNames.Remove(CurrentColumnName!.Value.Key);
            CurrentColumnNames!.Remove(CurrentColumnName.Value);
            RefreshCommand.NotifyCanExecuteChanged();
        }

        private bool CanRemoveColumnName() => CurrentColumnName is not null && SelectedChartSetting is not null && SelectedChartSetting.ColumnNames.ContainsKey(CurrentColumnName.Value.Key);

        //[RelayCommand(CanExecute = nameof(CanRefresh))]
        [RelayCommand]
        public void Refresh()
        {
            var type = PlotControl.Plot.Add.GetType();

            PlotControl.Plot.Clear();

            if (ChartSettings.Count > 0)
                foreach (var setting in ChartSettings)
                {
                    if (setting.ColumnNames.Count == 0 || setting.TypeSetting!.AddItem is null)
                        continue;

                    var axisSetting = new ChartAxisSetting();
                    if (setting.FillColor is not null)
                        axisSetting.FillColor = setting.FillColor.Value.ARGB();
                    if (setting.LineColor is not null)
                        axisSetting.LineColor = setting.LineColor.Value.ARGB();

                    axisSetting.DataPath = setting.ColumnNames.Select(kv => kv.Key).ToList();

                    setting.TypeSetting.AddItem(MasterData!, axisSetting, PlotControl.Plot.Add);
                }

            if (!string.IsNullOrEmpty(XLabel))
                PlotControl.Plot.XLabel(XLabel);

            if (!string.IsNullOrEmpty(YLabel))
                PlotControl.Plot.YLabel(YLabel);

            PlotControl.Plot.Axes.AutoScaleX();
            PlotControl.Plot.Axes.AutoScaleY();

            //PlotControl.Plot.ShowLegend();
            PlotControl.Refresh();
        }

        //private bool CanRefresh() => ChartSettings.Count > 0;

        partial void OnMasterDataChanged(IEnumerable? value)
        {
            if (value is null || value.Cast<DataRowView>().Count() == 0)
                return;

            columnNames.Clear();
            foreach (DataColumn column in value.Cast<DataRowView>().First().DataView.Table!.Columns)
                columnNames.Add(column.ColumnName, column.Caption);
        }

        partial void OnTitleChanged(string value) => PlotControl.Plot.Title(value);

        partial void OnSelectedChartSettingChanged(ChartSetting? value)
        {
            if (value is null)
                CurrentColumnNames = null;
            else
                // 这里使用 ObservableCollection<string> 是为了支持 WPF 数据绑定
                // 直接使用 IEnumerable<string> 会导致无法在 XAML 中绑定到 ItemsSource
                CurrentColumnNames = new ObservableCollection<KeyValuePair<string, string>>(value?.ColumnNames.ToArray()!);
        }
    }
}
