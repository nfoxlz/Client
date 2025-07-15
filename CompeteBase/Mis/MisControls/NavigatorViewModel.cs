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
using System.Windows.Input;

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

        [ObservableProperty]
        private ICommand? _pageSizeChangingCommand;

        [ObservableProperty]
        private object? _pageSizeChangingCommandParameter;

        [ObservableProperty]
        private ICommand? _pageSizeChangedCommand;

        [ObservableProperty]
        private object? _pageSizeChangedCommandParameter;

        [ObservableProperty]
        private ICommand? _currentPageNoChangingCommand;

        [ObservableProperty]
        private object? _currentPageNoChangingCommandParameter;

        [ObservableProperty]
        private ICommand? _currentPageNoChangedCommand;

        [ObservableProperty]
        private object? _currentPageNoChangedCommandParameter;

        /// <summary>
        /// 获取或设置一个值，该值指示记录总数。
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MaxPageNo), nameof(CurrentPageNo))]
        [NotifyCanExecuteChangedFor(nameof(FirstCommand), nameof(PreviousCommand), nameof(NextCommand), nameof(LastCommand))]
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
                    if (_pageSize == value)
                        return;

                    if (PageSizeChangingFunc?.Invoke(_pageSize, value) == true)
                        return;

                    if (PageSizeChangingCommand?.CanExecute(PageSizeChangingCommandParameter) == true)
                        PageSizeChangingCommand?.Execute(PageSizeChangingCommandParameter);

                    var oldValue = _pageSize;
                    _pageSize = value;
                    NotifyPropertyChanged();

                    PageSizeChangedAction?.Invoke(oldValue, _pageSize);

                    if (PageSizeChangedCommand is null)
                    {
                        if (PageSizeChangedAction is null)
                        {
                            CurrentPageNo = CurrentPageNo * (oldValue - 1UL) / _pageSize + 1UL;
                            //if (CurrentPageNo > MaxPageNo)
                            //    CurrentPageNo = MaxPageNo;
                            this.NotifyPropertyChanged(p => p.CurrentPageNo);
                        }
                    }
                    else if (PageSizeChangedCommand?.CanExecute(PageSizeChangedCommandParameter) == true)
                        PageSizeChangedCommand?.Execute(PageSizeChangedCommandParameter);

                    OnPropertyChanged(nameof(MaxPageNo));
                    NotifyButtonCommand();
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
                if (_currentPageNo == value || value > MaxPageNo)
                    return;

                if (CurrentPageNoChangingFunc?.Invoke(_currentPageNo, value) == true)
                    return;

                if (CurrentPageNoChangingCommand?.CanExecute(CurrentPageNoChangingCommand) == true)
                    CurrentPageNoChangingCommand?.Execute(CurrentPageNoChangingCommand);

                var oldValue = _currentPageNo;
                _currentPageNo = value;
                NotifyPropertyChanged();

                CurrentPageNoChangedAction?.Invoke(oldValue, _currentPageNo);

                if (CurrentPageNoChangedCommand?.CanExecute(CurrentPageNoChangedCommandParameter) == true)
                    CurrentPageNoChangedCommand?.Execute(CurrentPageNoChangedCommandParameter);

                NotifyButtonCommand();
            }
        }

        private void NotifyButtonCommand()
        {
            FirstCommand.NotifyCanExecuteChanged();
            PreviousCommand.NotifyCanExecuteChanged();
            NextCommand.NotifyCanExecuteChanged();
            LastCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty]
        private IEnumerable<ushort> _pageCollection = Constants.DefaultNavigatorPageCollection;

        [RelayCommand(CanExecute = nameof(CanPrevious))]
        private void First() => CurrentPageNo = 1UL;

        [RelayCommand(CanExecute = nameof(CanPrevious))]
        private void Previous() => CurrentPageNo--;

        [RelayCommand(CanExecute = nameof(CanNext))]
        private void Next() => CurrentPageNo++;

        [RelayCommand(CanExecute = nameof(CanNext))]
        private void Last() => CurrentPageNo = MaxPageNo;

        private bool CanPrevious() => CurrentPageNo > 1UL;

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
