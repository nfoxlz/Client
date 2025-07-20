using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Compete.Extensions;
using Compete.Mis.MisControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Layout;

namespace Compete.Mis.Plugins
{
    public abstract partial class DataViewModel : PluginViewModel
    {
        [ObservableProperty]
        private BindingListCollectionView? _masterData;

        [ObservableProperty]
        private DataTable? _totalTable;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ConditionData))]
        private DataTable? _conditionTable;

        [ObservableProperty]
        private BindingListCollectionView? _conditionData = null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MasterData))]
        [NotifyPropertyChangedFor(nameof(TotalTable))]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExtendedSaveCommand))]
        [NotifyCanExecuteChangedFor(nameof(SaveCloseCommand))]
        [NotifyCanExecuteChangedFor(nameof(SaveCallBackCommand))]
        private DataSet? _data = null;

        [ObservableProperty]
        private string? _messageText;// = "就绪";

        [ObservableProperty]
        private string? _sortDescription;

        [ObservableProperty]
        private ushort _pageSize;

        [ObservableProperty]
        private ulong _currentPageNo;

        [ObservableProperty]
        private ulong _recordCount;

        [ObservableProperty]
        private Guid? _actionId;

        [ObservableProperty]
        private IEnumerable<Chart.ChartSetting>? _chartSettings;

        [ObservableProperty]
        private string? _xLabel;

        [ObservableProperty]
        private string? _yLabel;


        public object? View { get; set; }

        protected void SetViewEnabled(bool isEnabled)
        {
            if (View is UIElement element)
                element.IsEnabled = isEnabled;
        }

        [RelayCommand]
        private void Cancel()
        {
            (View as Window)?.Close();

            if (PluginParameter?.BackCallAction is not null)
                PluginParameter.BackCallAction(false);
        }


        private ulong runCount = 0UL;

        private readonly Lock runCountLock = new();

        private void RunBackground(Action action)
        {
            Cursor? cursor = null;
            lock (runCountLock)
            {
                if (0UL == runCount)
                {
                    SetViewEnabled(false);
                    cursor = ViewHelper.BeginProcess();
                }
                runCount++;
            }

            try
            {
                action();
            }
            catch (Exceptions.BusinessException exception)
            {
                MessageText = exception.Message;
                MessageDialog.Error(MessageText);
            }
            catch (Exception exception)
            {
                MessageText = exception.Message;
                MessageDialog.Exception(exception);
            }
            finally
            {
                lock (runCountLock)
                {
                    runCount--;
                    if (0UL == runCount)
                    {
                        ViewHelper.EndProcess(cursor);
                        SetViewEnabled(true);
                    }
                }
            }
        }

        protected override bool CheckAuthorition(long authorition) => CheckAuthorition(authorition, MasterData?.CurrentItem);

        protected virtual bool CheckAuthorition(long authorition, object? item) => true;

        protected override bool GetRunAuthorition(PluginCommandParameter? parameter) => GetRunAuthorition(parameter, MasterData?.CurrentItem);

        protected virtual bool GetRunAuthorition(PluginCommandParameter? parameter, object? item) => true;

        protected abstract DataSet? QueryData(string? name);

        protected override Action<bool>? BackCallAction
        {
            get => (isSucceed) =>
            {
                if (isSucceed && CanQuery())
                    Query();
            };
        }

        public event EventHandler<EventArgs>? Queried;

        protected virtual void OnQueried(EventArgs e) => Volatile.Read(ref Queried)?.Invoke(this, e);

        public bool HasQueryAuthorition { get => HasAuthorition(ReserveAuthorition.Query); }

        private void SaveCommandNotifyCanExecuteChanged()
        {
            SaveCommand.NotifyCanExecuteChanged();
            ExtendedSaveCommand.NotifyCanExecuteChanged();
            SaveCloseCommand.NotifyCanExecuteChanged();
            SaveCallBackCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanQuery))]
        private void Query(string? name = null) => RunBackground(() =>
        {
            ConditionData?.CommitEdit();

            var data = QueryData(name);
            if (data != null)
            {
                data.AcceptChanges();
                Data = data;
            }

            ActionId = Guid.NewGuid();

            NotifyCanExecuteChanged();

            OnQueried(new EventArgs());

            //SaveCommandNotifyCanExecuteChanged();
        });


        protected virtual bool CanQuery()
        {
            if (!HasQueryAuthorition)
                return false;

            if (ConditionTable is not null && ConditionTable.Verify(out string viewErrorText, false))
            {
                MessageText = viewErrorText;
                return false;
            }

            MessageText = string.Empty;
            return true;
        }

        [RelayCommand(CanExecute = nameof(CanQuery))]
        private void ShowChart()
        {
            //if (Data is null || Data.Tables.Count == 0)
            //    return;
            if (MasterData is null)
                Query();

            if (Data is null)
                return;

            var chart = new Chart.ChartView();
            var vm = (chart.DataContext as Chart.ChartViewModel)!;
            vm.Title = PluginParameter?.Title ?? string.Empty;
            vm.MasterData = MasterData;

            vm.XLabel = XLabel;
            vm.YLabel = YLabel;

            if (ChartSettings is not null)
            {
                var columns = Data.Tables[0].Columns;
                foreach (var setting in ChartSettings)
                {
                    foreach (var name in setting.ColumnNames)
                        if (columns.Contains(name.Key))
                            setting.ColumnNames[name.Key] = columns[name.Key]!.Caption;
                    vm.ChartSettings.Add(setting);
                }

                if (vm.ChartSettings.Count > 0)
                {
                    vm.SelectedChartSetting = vm.ChartSettings[0];
                    vm.SettingWidth = new GridLength(0, GridUnitType.Star);
                    vm.Refresh();
                }
            }

            var document = new LayoutDocument() { Content = chart, Title = GlobalCommon.GetMessage("Chart.Title", PluginParameter?.Title??string.Empty) };
            GlobalCommon.MainDocumentPane!.Children.Add(document);
            GlobalCommon.MainDocumentPane.SelectedContentIndex = GlobalCommon.MainDocumentPane.Children.Count - 1;
        }

        partial void OnMasterDataChanged(BindingListCollectionView? oldValue, BindingListCollectionView? newValue)
        {
            if (oldValue is not null)
                oldValue.CurrentChanged -= DataViewModel_CurrentChanged;

            if (newValue is not null)
                newValue.CurrentChanged += DataViewModel_CurrentChanged;
        }

        private void DataViewModel_CurrentChanged(object? sender, EventArgs e)
        {
            RunCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }

        public bool HasModifyAuthorition { get => HasAuthorition(ReserveAuthorition.Modify); }

        [RelayCommand(CanExecute = nameof(CanAdd))]
        private void Add()
        {
            MasterData!.AddNew();
            MasterData.CommitNew();
            SaveCommandNotifyCanExecuteChanged();
        }

        public bool HasAddAuthorition { get => HasAuthorition(ReserveAuthorition.Add); }

        private bool CanAdd()
        {
            if (!HasAddAuthorition || MasterData is null)// && MasterData.CanAddNew;
                return false;

            foreach (var item in MasterData)
                if (item.GetType().ToString() == "MS.Internal.NamedObject")
                    return false;

            return true;
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private void Delete()
        {
            MasterData!.Remove(MasterData.CurrentItem);
            SaveCommandNotifyCanExecuteChanged();
        }

        public bool HasDeleteAuthorition { get => HasAuthorition(ReserveAuthorition.Delete); }

        private bool CanDelete() => HasDeleteAuthorition && MasterData is not null && MasterData.CurrentItem is not null && MasterData.CurrentItem.GetType().ToString() != "MS.Internal.NamedObject";// && MasterData.CanRemove;

        private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            OnPropertyChanged(nameof(Data));
            SaveCommandNotifyCanExecuteChanged();
        }

        private void NotifyCanExecuteChanged()
        {
            AddCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }

        protected virtual bool SaveData(string? name) => false;

        private bool isPaused;

        private readonly Lock pausedLock = new();

        private void SaveData(string? name, Action action)
        {
            if (isPaused)
                return;

            lock (pausedLock)
                if (isPaused)
                    return;
                else
                    isPaused = true;

            try
            {
                if (SaveData(name))
                    return;

                //ActionId = Guid.NewGuid();
                action();
            }
            finally
            {
                isPaused = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save(RoutedEventArgs args) => args.Run<string>(Save);
        //{
        //    args.Run<string>(Save);
        //    var source = args.GetCommandSource();
        //    var element = source as UIElement;
        //    if (element != null)
        //        element.IsEnabled = false;
        //    try
        //    {
        //        Save(source?.CommandParameter.ToString());
        //    }
        //    finally
        //    {
        //        if (element != null)
        //            element.IsEnabled = true;
        //    }
        //}

        private void Save(string? name) => RunBackground(() =>
            SaveData(name, () =>
            {
                Data!.AcceptChanges();

                if (CanQuery())
                    Query();
            }));

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void SaveClose(string? name) => RunBackground(() =>
            SaveData(name, () =>
            {
                if (PluginParameter?.BackCallAction is not null)
                    Threading.ThreadHelper.RunBackgroundUI(() => PluginParameter.BackCallAction(true));

                (View as Window)?.Close();
            }));

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void SaveCallBack(string? name) => RunBackground(() =>
            SaveData(name, () =>
            {
                if (PluginParameter?.BackCallAction is not null)
                    Threading.ThreadHelper.RunBackgroundUI(() => PluginParameter.BackCallAction(true));

                Data!.AcceptChanges();

                if (CanQuery())
                    Query();
            }));

        public bool HasSaveAuthorition { get => HasAuthorition(ReserveAuthorition.Save); }

        protected virtual DataSet? GetSaveData() => Data;

        private bool VerifyNonnullData()
        {
            var builder = new StringBuilder();
            if (View is DependencyObject view && view.Verify(out string viewErrorText))
                builder.Append(viewErrorText);

            var saveData = GetSaveData();
            if (saveData != null && saveData.Verify(out string dataErrorText))
                builder.Append(dataErrorText);

            MessageText = builder.ToString();
            return Verify() && string.IsNullOrWhiteSpace(MessageText);
        }

        private bool VerifyData() => Data is not null && Data.HasChanges() && !isPaused && VerifyNonnullData();//Data is null || !Data.HasChanges() || isPaused ? false : VerifyNonnullData()

        private bool VerifyNoChangeData() => Data is not null && !isPaused && VerifyNonnullData();// Data is null || isPaused ? false : VerifyNonnullData()

        protected virtual bool CanSave() => HasSaveAuthorition && VerifyData();

        public bool HasExtendedSaveAuthorition { get => HasAuthorition(ReserveAuthorition.ExtendedSave); }

        private bool CanExtendedSave(ExtendedSaveParameter? parameter) => HasAuthorition((long)ReserveAuthorition.ExtendedSave << (parameter?.AuthoritionFlag ?? 0)) && ((parameter?.Availability ?? false) && VerifyNoChangeData() || VerifyData());

        [RelayCommand(CanExecute = nameof(CanExtendedSave))]
        private void ExtendedSave(ExtendedSaveParameter? parameter) => Save(parameter?.Name ?? "extendedSave");

        [RelayCommand(CanExecute = nameof(CanAudit))]
        private void Audit(string? name) => DirectExecuteSaveSql(name ?? "audit");

        [RelayCommand(CanExecute = nameof(CanAudit))]
        private void AuditClose(string? name) => DirectExecuteSaveSqlClose(name ?? "audit");

        public bool HasAuditAuthorition { get => HasAuthorition(ReserveAuthorition.Audit); }

        private bool CanAudit() => HasAuditAuthorition;// && VerifyData()

        [RelayCommand(CanExecute = nameof(CanDirectSave))]
        private void DirectSave(string? name) => DirectExecuteSaveSql(name ?? "save");

        public bool HasDirectSaveAuthorition { get => HasAuthorition(ReserveAuthorition.DirectSave); }

        private bool CanDirectSave() => HasDirectSaveAuthorition && CanDirectExecuteSql;

        [RelayCommand(CanExecute = nameof(CanDirectAdd))]
        private void DirectAdd(string? name) => DirectExecuteSaveSql(name ?? "add");

        public bool HasDirectAddAuthorition { get => HasAuthorition(ReserveAuthorition.DirectAdd); }

        private bool CanDirectAdd() => HasDirectAddAuthorition && CanDirectExecuteSql;

        [RelayCommand(CanExecute = nameof(CanDirectDelete))]
        private void DirectDelete(string? name) => DirectExecuteSaveSql(name ?? "delete");

        [RelayCommand(CanExecute = nameof(CanDirectDelete))]
        private void DirectDeleteClose(string? name) => DirectExecuteSaveSqlClose(name ?? "delete");

        public bool HasDirectDeleteAuthorition { get => HasAuthorition(ReserveAuthorition.DirectDelete); }

        private bool CanDirectDelete() => HasDirectDeleteAuthorition && CanDirectExecuteSql;

        [RelayCommand(CanExecute = nameof(CanDirectModify))]
        private void DirectModify(string? name) => DirectExecuteSaveSql(name ?? "modify");

        public bool HasDirectModifyAuthorition { get => HasAuthorition(ReserveAuthorition.DirectModify); }

        private bool CanDirectModify() => HasDirectModifyAuthorition && CanDirectExecuteSql;

        [RelayCommand(CanExecute = nameof(CanDirectAddChild))]
        private void DirectAddChild(string? name) => DirectExecuteSaveSql(name ?? "addChild");

        public bool HasDirectAddChildAuthorition { get => HasAuthorition(ReserveAuthorition.DirectAddChild); }

        private bool CanDirectAddChild() => HasDirectAddChildAuthorition && CanDirectExecuteSql;

        [RelayCommand(CanExecute = nameof(CanDirectDeleteChild))]
        private void DirectDeleteChild(string? name) => DirectExecuteSaveSql(name ?? "deleteChild");

        public bool HasDirectDeleteChildAuthorition { get => HasAuthorition(ReserveAuthorition.DirectDeleteChild); }

        private bool CanDirectDeleteChild() => HasDirectDeleteChildAuthorition && CanDirectExecuteSql;

        private void DirectExecuteSaveSqlAction(string name, Action action)
        {
            if (isPaused)
                return;

            if (MessageDialog.Warning("Message.ExecuteSaveSqlWarning", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            lock (pausedLock)
                if (isPaused)
                    throw new Exceptions.LockException();
                else
                    isPaused = true;

            try
            {
                if (ExecuteSaveSql(name))
                    return;

                action();
            }
            catch(Exceptions.LockException)
            {
                MessageDialog.Warning("Message.LockWarning");
            }
            finally
            {
                isPaused = false;
            }
        }

        private void DirectExecuteSaveSql(string name) => RunBackground(() =>
            DirectExecuteSaveSqlAction(name, () =>
            {
                if (CanQuery())
                    Query();
            }));

        private void DirectExecuteSaveSqlClose(string name) => RunBackground(() =>
            DirectExecuteSaveSqlAction(name, () =>
            {
                if (PluginParameter?.BackCallAction is not null)
                    Threading.ThreadHelper.RunBackgroundUI(() => PluginParameter.BackCallAction(true));
                    //Task.Run(() => Application.Current.Dispatcher.Invoke(() => PluginParameter.BackCallAction(true)));

                (View as Window)?.Close();
            }));

        protected virtual bool ExecuteSaveSql(string name) => false;

        protected virtual bool CanDirectExecuteSql => true;

        protected bool VerifyConditionTable()
        {
            if (ConditionTable is null)
                return false;

            if (ConditionTable.Verify(out string dataErrorText, false))
            {
                MessageText += dataErrorText;
                return false;
            }

            return true;
        }

        protected virtual FlowDocument? GetDocument() => null;

        private FlowDocument? GetDataFlowDocument()
        {
            var document = GetDocument();
            if (document is null)
                return null;

            document.DataContext = this;

            return document;
        }

        public bool HasPrintAuthorition { get => HasAuthorition(ReserveAuthorition.Print); }

        private bool CanPrint => HasPrintAuthorition;

        [RelayCommand(CanExecute = nameof(CanPrint))]
        private void Print()
        {
            var document = GetDataFlowDocument();
            if (document is null)
                return;

            var viewer = new DocumentViewer();
            Dispatcher.CurrentDispatcher.BeginInvoke(PrintHelper.LoadXps, DispatcherPriority.ApplicationIdle, viewer, GetDocument());
            viewer.Print();
        }

        [RelayCommand(CanExecute = nameof(CanPrint))]
        private void PrintPreview()
        {
            var document = GetDataFlowDocument();
            if (document is null)
                return;

            var window = new Print.PrintPreviewWindow();
            Dispatcher.CurrentDispatcher.BeginInvoke(PrintHelper.LoadXps, DispatcherPriority.ApplicationIdle, window.documentViewer, document);
            window.Show();
        }

        protected virtual bool Verify() => true;

        protected virtual bool Initializing() => false;

        protected bool IsInitializing = false;

        protected virtual void Initialized() { }

        public void Initialize()
        {
            IsInitializing = true;

            try
            {
                if (Initializing())
                    return;

                if (CanQuery())
                    Query(null);

                Initialized();
            }
            finally
            {
                IsInitializing = false;
            }

            QueryCommand.NotifyCanExecuteChanged();
        }

        partial void OnDataChanged(DataSet? oldValue, DataSet? newValue)
        {
            if (MasterData is not null)
                MasterData.CurrentChanged -= MasterData_CurrentChanged;

            if (oldValue is not null)
                foreach (DataTable table in oldValue.Tables)
                {
                    table.ColumnChanged -= Table_ColumnChanged;
                    table.RowDeleted -= Table_RowChanged;
                    table.TableNewRow -= Table_TableNewRow;
                }

            if (newValue is not null)
            {
                MasterData = CollectionViewSource.GetDefaultView(newValue.Tables[0]) as BindingListCollectionView;      // 获取主数据视图。
                if (MasterData is not null)
                    MasterData.CurrentChanged += MasterData_CurrentChanged;

                foreach (DataTable table in newValue.Tables)
                {
                    table.ColumnChanged += Table_ColumnChanged;
                    table.RowDeleted += Table_RowChanged;
                    table.TableNewRow += Table_TableNewRow;
                }
            }
        }

        private void MasterData_CurrentChanged(object? sender, EventArgs e)
        {
            RunCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            DirectDeleteCommand.NotifyCanExecuteChanged();
            DirectModifyCommand.NotifyCanExecuteChanged();
            AuditCommand.NotifyCanExecuteChanged();
            DirectAddChildCommand.NotifyCanExecuteChanged();
            DirectDeleteChildCommand.NotifyCanExecuteChanged();
        }

        protected virtual void Table_TableNewRow(object sender, DataTableNewRowEventArgs e) { }

        private void Table_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            e.Row.EndEdit();
            SaveCommandNotifyCanExecuteChanged();
        }

        partial void OnConditionTableChanged(DataTable? oldValue, DataTable? newValue)
        {
            if (oldValue is not null)
            {
                oldValue.RowChanged -= ConditionTable_RowChanged;
                oldValue.RowDeleted -= ConditionTable_RowChanged;
            }

            if (newValue is not null)
            {
                ConditionData = CollectionViewSource.GetDefaultView(newValue) as BindingListCollectionView;
                newValue.RowChanged += ConditionTable_RowChanged;
                newValue.RowDeleted += ConditionTable_RowChanged;
            }

            ConditionTableChanged(oldValue, newValue);
        }

        protected virtual void ConditionTableChanged(DataTable? oldValue, DataTable? newValue) { }

        private void ConditionTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            QueryCommand.NotifyCanExecuteChanged();

            DirectAddCommand.NotifyCanExecuteChanged();
            DirectDeleteCommand.NotifyCanExecuteChanged();
            DirectModifyCommand.NotifyCanExecuteChanged();

            DirectAddChildCommand.NotifyCanExecuteChanged();
            DirectDeleteChildCommand.NotifyCanExecuteChanged();
        }

        protected override bool CanRun(PluginCommandParameter parameter) => base.CanRun(parameter) && (parameter?.RequiredCurrentItem == false || MasterData?.CurrentItem is not null);
    }
}
