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
using System.Xml.Linq;

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

        private ulong runCount = 0UL;

        private readonly object runCountLock = new();

        private void RunBackground(Action action)
        {
            Cursor? cursor = null;
            lock (runCountLock)
            {
                if (runCount == 0UL)
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
                    if (runCount == 0UL)
                    {
                        ViewHelper.EndProcess(cursor);
                        SetViewEnabled(true);
                    }
                }
            }
        }
        protected abstract void QueryData(string? name);

        public event EventHandler<EventArgs>? Queried;

        protected virtual void OnQueried(EventArgs e) => Volatile.Read(ref Queried)?.Invoke(this, e);

        public bool HasQueryAuthorition { get => HasAuthorition(ReserveAuthorition.Query); }

        [RelayCommand(CanExecute = nameof(CanQuery))]
        private void Query(string? name) => RunBackground(() =>
        {
            ConditionData?.CommitEdit();

            QueryData(name);
            if (Data != null)
            {
                Data.AcceptChanges();

                foreach (DataTable table in Data.Tables)
                {
                    table.RowChanged += Table_RowChanged;
                    table.RowDeleted += Table_RowChanged;
                }
                //if (MasterData != null)
                //    MasterData.CurrentChanged += DataViewModel_CurrentChanged;
            }

            ActionId = Guid.NewGuid();

            NotifyCanExecuteChanged();

            OnQueried(new EventArgs());

            SaveCommand.NotifyCanExecuteChanged();
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
            SaveCommand.NotifyCanExecuteChanged();
        }

        public bool HasAddAuthorition { get => HasAuthorition(ReserveAuthorition.Add); }

        private bool CanAdd()
        {
            if (!HasAddAuthorition || MasterData == null)// && MasterData.CanAddNew;
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
            SaveCommand.NotifyCanExecuteChanged();
        }

        public bool HasDeleteAuthorition { get => HasAuthorition(ReserveAuthorition.Delete); }

        private bool CanDelete() => HasDeleteAuthorition && MasterData != null && MasterData.CurrentItem != null && MasterData.CurrentItem.GetType().ToString() != "MS.Internal.NamedObject";// && MasterData.CanRemove;

        //private void Table_TableNewRow(object sender, DataTableNewRowEventArgs e) => SaveCommand.NotifyCanExecuteChanged();

        private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            SaveCommand.NotifyCanExecuteChanged();
            NotifyCanExecuteChanged();
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

        public bool HasSaveAuthorition { get => HasAuthorition(ReserveAuthorition.Save); }

        private bool VerifyData()
        {
            if (Data == null || !Data.HasChanges() || isPaused)
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
                    Query(null);
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
            if (ConditionTable == null)
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
            if (document == null)
                return null;

            //var data = new DataSet();

            //if (Data != null)
            //    data.Merge(Data);

            //if (ConditionTable != null)
            //    data.Merge(ConditionTable);

            //document.DataContext = data;
            document.DataContext = this;

            return document;
        }

        public bool HasPrintAuthorition { get => HasAuthorition(ReserveAuthorition.Print); }

        private bool CanPrint => HasPrintAuthorition;

        [RelayCommand(CanExecute = nameof(CanPrint))]
        private void Print()
        {
            var document = GetDataFlowDocument();
            if (document == null)
                return;

            var viewer = new DocumentViewer();
            Dispatcher.CurrentDispatcher.BeginInvoke(PrintHelper.LoadXps, DispatcherPriority.ApplicationIdle, viewer, GetDocument());
            viewer.Print();
        }

        [RelayCommand(CanExecute = nameof(CanPrint))]
        private void PrintPreview()
        {
            //var doc = (FlowDocument)XamlReader.Parse(File.ReadAllText("D:/Projects/CompeteMIS/tmp/TestFlowDocument.xaml"));
            ////var paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
            ////paginator.PageSize = new Size(595, 842);
            ////doc.ColumnWidth = double.PositiveInfinity;

            //var window = new Print.PrintPreviewWindow();

            ////Dispatcher.CurrentDispatcher.BeginInvoke(new PrintHelper.LoadXpsMethod(PrintHelper.LoadXps), DispatcherPriority.ApplicationIdle, window.documentViewer, doc);
            //Dispatcher.CurrentDispatcher.BeginInvoke(Print.PrintHelper.LoadXps, DispatcherPriority.ApplicationIdle, window.documentViewer, doc);
            ////PrintHelper.LoadXps(doc, window.docViewer);

            //window.Show();

            var document = GetDataFlowDocument();
            if (document == null)
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

        //partial void OnDataChanging(DataSet? oldValue, DataSet? newValue)
        //{
        //    if (oldValue != null)
        //        foreach (DataTable table in oldValue.Tables)
        //        {
        //            table.ColumnChanging -= Table_ColumnChanging;
        //            table.RowChanging -= Table_RowChanging;
        //            table.RowChanged -= Table_RowChanged;
        //        }
        //}

        //private void Table_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        //{
        //}

        //private void Table_RowChanging(object sender, DataRowChangeEventArgs e)
        //{
        //}

        //private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
        //{
        //}

        partial void OnDataChanged(DataSet? value)
        {
            if (value != null)
                MasterData = CollectionViewSource.GetDefaultView(value.Tables[0]) as BindingListCollectionView;      // 获取主数据视图。
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
        }

        public override bool HasRunAuthorition => base.HasRunAuthorition && MasterData?.CurrentItem != null;
    }
}
