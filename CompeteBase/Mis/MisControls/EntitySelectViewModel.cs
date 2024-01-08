// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/12 14:41:11 LeeZheng 新建。
//==============================================================
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Compete.Mis.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// EntitySelectViewModel 类。
    /// </summary>
    internal sealed partial class EntitySelectViewModel : DialogViewModel
    {
        /// <summary>
        /// 获取或设置服务参数。
        /// </summary>
        public string? ServiceParameter { get; set; }

        public IDictionary<string, object>? Conditions { get; set; }

        /// <summary>
        /// 获取或设置选定项目。
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private object? _selectedItem;

        [ObservableProperty]
        private IEnumerable? _masterData;

        private string? _filter;

        /// <summary>
        /// 获取或设置过滤条件。
        /// </summary>
        public string? Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                QueryData();
            }
        }

        public string? FilterFormat { get; set; }

        [ObservableProperty]
        private ulong _recordCount;

        private ushort _pageSize = 15;

        /// <summary>
        /// 获取或设置一个值，用于指示在分页处理数据时，每页的最大数据记录数。
        /// </summary>
        public ushort PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value;
                QueryData();
            }
        }

        private ulong _currentPageNo;

        /// <summary>
        /// 获取或设置一个值，用于指示在分页处理数据时，当前的页号。
        /// 索引值从1开始。
        /// </summary>
        public ulong CurrentPageNo
        {
            get => _currentPageNo;
            set
            {
                _currentPageNo = value;
                QueryData();
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ClearButtonVisibility))]
        private bool _isRequired;

        public Visibility ClearButtonVisibility => IsRequired ? Visibility.Hidden : Visibility.Visible;

        private bool isPaused;

        /// <summary>
        /// 查询数据。
        /// </summary>
        public void QueryData()
        {
            if (isPaused || string.IsNullOrWhiteSpace(ServiceParameter))
                return;

            //Threading.ThreadingHelper.Invoke(() =>
            //{
            //    var (data, count) = Global.EntityDataProvider.Query(ServiceParameter, string.IsNullOrWhiteSpace(FilterFormat) ? string.Empty : string.Format(FilterFormat, Filter), CurrentPageNo, PageSize);  // 取得数据。
            //    if (count == 0)
            //        return;

            //    DataSettingManager.Setup(data);
            //    MasterData = CollectionViewSource.GetDefaultView(data.Tables[0]);
            //}, "Query");
            //var (data, count) = GlobalCommon.EntityDataProvider!.Query(ServiceParameter, Conditions, string.IsNullOrWhiteSpace(FilterFormat) ? string.Empty : string.Format(FilterFormat, Filter), CurrentPageNo, PageSize);  // 取得数据。
            Conditions ??= new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(Filter))
                Conditions.Add("filter", Filter);
            var result = GlobalCommon.EntityDataProvider!.Query(ServiceParameter, Conditions, CurrentPageNo, PageSize);  // 取得数据。
            if (result.Count == 0)
                return;

            var idName = $"{ServiceParameter}_Id";
            if (result.Data!.Tables[0].Columns.Contains(idName))
                result.Data.Tables[0].Columns[idName]!.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsVisible] = false;

            //DataSettingManager.Setup(data);
            isPaused = true;
            RecordCount = result.Count;
            CurrentPageNo = result.PageNo;
            isPaused = false;

            MasterData = CollectionViewSource.GetDefaultView(result.Data.Tables[0]);
        }

        /// <summary>
        /// OK 命令的CanExecute事件的处理程序。
        /// </summary>
        /// <param name="sender">调用处理程序的命令目标。</param>
        /// <param name="e">事件数据。</param>
        protected override bool CanOk() => SelectedItem != null;

        /// <summary>
        /// OK 命令的Executed事件的处理程序。
        /// </summary>
        /// <param name="sender">事件处理程序所附加到的对象。</param>
        /// <param name="e">事件数据。</param>
        protected override void DoOk(FrameworkElement sender) => sender.GetWindow()!.DialogResult = true;

        [RelayCommand]
        private void Clear(FrameworkElement sender)
        {
            SelectedItem = null;
            sender.GetWindow()!.DialogResult = true;
        }

        [RelayCommand(CanExecute = nameof(CanRefresh))]
        private void Refresh() => QueryData();

        private bool CanRefresh() => ServiceParameter != null;
    }
}
