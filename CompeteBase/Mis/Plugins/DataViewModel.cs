using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Compete.Mis.MisControls;
using System;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

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
        [NotifyCanExecuteChangedFor(nameof(SaveCloseCommand))]
        private DataSet? _data = null;

        [ObservableProperty]
        private string? _messageText;// = "就绪";

        [ObservableProperty]
        private ushort _pageSize;

        [ObservableProperty]
        private ulong _currentPageNo;

        [ObservableProperty]
        private ulong _recordCount;

        [ObservableProperty]
        private Guid? _actionId;

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

            if (PluginParameter?.BackAction != null)
                PluginParameter.BackAction(false);
        }


        private ulong runCount = 0UL;

        private readonly object runCountLock = new();

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
        protected abstract void QueryData(string? name);

        protected override Action<bool>? BackAction
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
            SaveCloseCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanQuery))]
        private void Query(string? name = null) => RunBackground(() =>
        {
            ConditionData?.CommitEdit();

            QueryData(name);
            Data?.AcceptChanges();

            ActionId = Guid.NewGuid();

            NotifyCanExecuteChanged();

            OnQueried(new EventArgs());

            SaveCommandNotifyCanExecuteChanged();
        });


        protected virtual bool CanQuery()
        {
            if (!HasQueryAuthorition)
                return false;

            if (ConditionTable != null && ConditionTable.Verify(out string viewErrorText))
            {
                MessageText = viewErrorText;
                return false;
            }

            MessageText = string.Empty;
            return true;
        }

        partial void OnMasterDataChanged(BindingListCollectionView? oldValue, BindingListCollectionView? newValue)
        {
            if (oldValue != null)
                oldValue.CurrentChanged -= DataViewModel_CurrentChanged;

            if (newValue != null)
                newValue.CurrentChanged += DataViewModel_CurrentChanged;
        }

        private void DataViewModel_CurrentChanged(object? sender, EventArgs e)
        {
            RunCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }

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
            if (!HasAddAuthorition || null == MasterData)// && MasterData.CanAddNew;
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

        private bool CanDelete() => HasDeleteAuthorition && MasterData != null && MasterData.CurrentItem != null && MasterData.CurrentItem.GetType().ToString() != "MS.Internal.NamedObject";// && MasterData.CanRemove;

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

        private readonly object pausedLock = new();

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save(string? name) => RunBackground(() =>
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

                Data!.AcceptChanges();

                if (CanQuery())
                    Query(null);
            }
            finally
            {
                isPaused = false;
            }
        });

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void SaveClose(string? name) => RunBackground(() =>
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

                (View as Window)?.Close();

                if (PluginParameter?.BackAction != null)
                    PluginParameter.BackAction(true);
            }
            finally
            {
                isPaused = false;
            }
        });

        public bool HasSaveAuthorition { get => HasAuthorition(ReserveAuthorition.Save); }

        private bool VerifyData()
        {
            if (null == Data || !Data.HasChanges() || isPaused)
                return false;

            var builder = new StringBuilder();
            if (View is DependencyObject view && view.Verify(out string viewErrorText))
                builder.Append(viewErrorText);

            if (Data.Verify(out string dataErrorText))
                builder.Append(dataErrorText);

            MessageText = builder.ToString();
            return Verify() && string.IsNullOrWhiteSpace(MessageText);
        }

        protected virtual bool CanSave() => HasSaveAuthorition && VerifyData();

        public bool HasExtendedSaveAuthorition { get => HasAuthorition(ReserveAuthorition.ExtendedSave); }

        private bool CanExtendedSave(ExtendedSaveParameter? parameter) => HasAuthorition((long)ReserveAuthorition.ExtendedSave << (parameter?.AuthoritionFlag ?? 0)) && VerifyData();

        [RelayCommand(CanExecute = nameof(CanExtendedSave))]
        private void ExtendedSave(ExtendedSaveParameter? parameter) => Save(parameter?.Name ?? "extendedSave");

        [RelayCommand(CanExecute = nameof(CanAudit))]
        private void Audit(string? name) => DirectExecuteSaveSql(name ?? "audit");

        public bool HasAuditAuthorition { get => HasAuthorition(ReserveAuthorition.Audit); }

        private bool CanAudit() => HasAuditAuthorition && VerifyData();
        
        [RelayCommand(CanExecute = nameof(CanDirectAdd))]
        private void DirectAdd(string? name) => DirectExecuteSaveSql(name ?? "add");

        public bool HasDirectAddAuthorition { get => HasAuthorition(ReserveAuthorition.DirectAdd); }

        private bool CanDirectAdd() => HasDirectAddAuthorition && CanDirectExecuteSql;

        [RelayCommand(CanExecute = nameof(CanDirectDelete))]
        private void DirectDelete(string? name) => DirectExecuteSaveSql(name ?? "delete");

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

        private void DirectExecuteSaveSql(string name) => RunBackground(() =>
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
                if (ExecuteSaveSql(name))
                    return;

                if (CanQuery())
                    Query();
            }
            finally
            {
                isPaused = false;
            }
        });

        protected virtual bool ExecuteSaveSql(string name) => false;

        protected virtual bool CanDirectExecuteSql => true;

        protected bool VerifyConditionTable()
        {
            if (null == ConditionTable)
                return false;

            if (ConditionTable.Verify(out string dataErrorText))
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
            if (null == document)
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
            if (null == document)
                return;

            var viewer = new DocumentViewer();
            Dispatcher.CurrentDispatcher.BeginInvoke(PrintHelper.LoadXps, DispatcherPriority.ApplicationIdle, viewer, GetDocument());
            viewer.Print();
        }

        [RelayCommand(CanExecute = nameof(CanPrint))]
        private void PrintPreview()
        {
            var document = GetDataFlowDocument();
            if (null == document)
                return;

            var window = new Print.PrintPreviewWindow();
            Dispatcher.CurrentDispatcher.BeginInvoke(PrintHelper.LoadXps, DispatcherPriority.ApplicationIdle, window.documentViewer, document);
            window.Show();
        }

        protected virtual bool Verify() => true;

        protected virtual bool Initializing() => false;

        protected bool IsInitializing = false;

        public void Initialize()
        {
            IsInitializing = true;

            try
            {
                if (Initializing())
                    return;

                if (CanQuery())
                    Query(null);
            }
            finally
            {
                IsInitializing = false;
            }

            QueryCommand.NotifyCanExecuteChanged();
        }

        partial void OnDataChanged(DataSet? oldValue, DataSet? newValue)
        {
            if (oldValue is not null)
                foreach (DataTable table in oldValue.Tables)
                {
                    table.ColumnChanged -= Table_ColumnChanged;
                    table.RowDeleted -= Table_RowChanged;
                }

            if (newValue is not null)
            {
                MasterData = CollectionViewSource.GetDefaultView(newValue.Tables[0]) as BindingListCollectionView;      // 获取主数据视图。
                foreach (DataTable table in newValue.Tables)
                {
                    table.ColumnChanged += Table_ColumnChanged;
                    table.RowDeleted += Table_RowChanged;
                }
            }
        }

        private void Table_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            e.Row.EndEdit();
            SaveCommandNotifyCanExecuteChanged();
        }

        partial void OnConditionTableChanged(DataTable? oldValue, DataTable? newValue)
        {
            if (oldValue != null)
            {
                oldValue.RowChanged -= ConditionTable_RowChanged;
                oldValue.RowDeleted -= ConditionTable_RowChanged;
            }

            if (newValue != null)
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

        protected override bool CanRun(PluginCommandParameter parameter) => base.CanRun(parameter) && (parameter?.RequiredCurrentItem == false || MasterData?.CurrentItem != null);
    }
}
