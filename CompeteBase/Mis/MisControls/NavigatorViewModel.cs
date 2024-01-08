// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/2/5 10:02:45 LeeZheng  新建。
// ===================================================================
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Compete.Mis.ViewModels;
using System;
using System.Collections.Generic;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// Navigator 视图模型类。
    /// </summary>
    internal sealed partial class NavigatorViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取一个值，该值指示最大页号，无数据时为“0”。
        /// </summary>
        public ulong MaxPageNo
        {
            get
            {
                var maxPageNo = RecordCount / PageSize;
                if (RecordCount % PageSize != 0UL || maxPageNo == 0UL)
                    maxPageNo++;

                return maxPageNo;
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示记录总数。
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MaxPageNo))]
        private ulong _recordCount;

        private ushort _pageSize = Constants.DefaultNavigatorPageSize;

        /// <summary>
        /// 获取或设置一个值，该值指示每页的记录数。
        /// </summary>
        public ushort PageSize
        {
            get => _pageSize;
            set
            {
                if (value > 0U)
                {
                    if (PageSizeChangingFunc?.Invoke(_pageSize, value) == true)
                        return;

                    if (_pageSize == value)
                        return;

                    var oldValue = _pageSize;
                    _pageSize = value;
                    if (CurrentPageNo > MaxPageNo)
                        CurrentPageNo = MaxPageNo;
                    this.NotifyPropertyChanged(p => p.MaxPageNo);

                    if (oldValue != _pageSize)
                        PageSizeChangedAction?.Invoke(oldValue, _pageSize);
                }
            }
        }

        private ulong _currentPageNo = 1UL;

        /// <summary>
        /// 获取或设置一个值，该值指示当前页号，值为1到MaxPageNo之间的数；当MaxPageNo为0时，值为1。
        /// </summary>
        public ulong CurrentPageNo
        {
            get => _currentPageNo;
            set
            {
                if (value > MaxPageNo)
                    return;

                if (CurrentPageNoChangingFunc?.Invoke(_currentPageNo, value) == true)
                    return;

                if (_currentPageNo == value)
                    return;

                var oldValue = _currentPageNo;
                _currentPageNo = value;
                NotifyPropertyChanged();

                if (oldValue != _currentPageNo)
                    CurrentPageNoChangedAction?.Invoke(oldValue, _currentPageNo);
            }
        }

        [ObservableProperty]
        private IEnumerable<ushort> _pageCollection = Constants.DefaultNavigatorPageCollection;

        [RelayCommand(CanExecute = nameof(CanPrevious))]
        private void First() => CurrentPageNo = 1L;

        [RelayCommand(CanExecute = nameof(CanPrevious))]
        private void Previous() => CurrentPageNo--;

        [RelayCommand(CanExecute = nameof(CanNext))]
        private void Next() => CurrentPageNo++;

        [RelayCommand(CanExecute = nameof(CanNext))]
        private void Last() => CurrentPageNo = MaxPageNo;

        private bool CanPrevious() => CurrentPageNo > 1L;

        private bool CanNext() => CurrentPageNo < MaxPageNo;
        
        /// <summary>
        /// 获取或设置每页的记录数改变中委托。
        /// </summary>
        public Func<ushort, ushort, bool>? PageSizeChangingFunc { get; set; }

        /// <summary>
        /// 获取或设置每页的记录数改变后委托。
        /// </summary>
        public Action<ushort, ushort>? PageSizeChangedAction { get; set; }

        /// <summary>
        /// 获取或设置当前页改变中委托。
        /// </summary>
        public Func<ulong, ulong, bool>? CurrentPageNoChangingFunc { get; set; }

        /// <summary>
        /// 获取或设置当前页改变后委托。
        /// </summary>
        public Action<ulong, ulong>? CurrentPageNoChangedAction { get; set; }
    }
}
