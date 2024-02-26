using Compete.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Compete.Mis.Plugins
{
    public sealed class SettingDataViewModel : SettingDataViewModel<DataPluginSetting> { }

    public abstract partial class SettingDataViewModel<T> : CustomSettingDataViewModel<T> where T : DataPluginSetting, new()
    {
        //public PluginCommandParameter? PluginParameter { get; set; }

        //public PluginSetting? Setting { get; set; }

        protected override bool Initializing()
        {
            if (string.IsNullOrWhiteSpace(PluginParameter!.Title))
                PluginParameter.Title = Setting?.Title ?? string.Empty;

            var basePath = Path.Combine(GlobalCommon.PluginPath, PluginParameter!.Path);
            Compile(basePath, Scripts.ScriptTemplates.CalculatorTemplate, Scripts.ScriptTemplates.CalculatorMethodTemplate, "Compete.Scripts.ConditionCalculator", Setting?.ConditionCalculateScriptFileNames, conditionCalculatorMethodDictionary);
            Compile(basePath, Scripts.ScriptTemplates.VerifyTemplate, Scripts.ScriptTemplates.VerifyMethodTemplate, "Compete.Scripts.ConditionVerifier", Setting?.ConditionVerifyScriptFileNames, conditionVerifierMethodDictionary);

            // 生成条件数据表。
            var conditionData = Setting?.ConditionMemoryDataSettings == null ? null : MemoryData.DataCreator.Create("ConditionTable", Setting.ConditionMemoryDataSettings);
            if (Setting?.ConditionMemoryData != null)
            {
                var data = MemoryData.DataCreator.Create(Setting.ConditionMemoryData);
                if (conditionData == null)
                    conditionData = data;
                else
                    conditionData.Merge(data);
            }

            if (conditionData != null)
            {
                if (Setting?.ConditionDataColumnSettings != null)
                    MemoryData.DataCreator.SetColumns(conditionData, Setting.ConditionDataColumnSettings);

                ConditionTable = conditionData;
                ConditionData?.AddNew();
                ConditionData?.CommitNew();
                if (Setting?.QueryParameters != null)
                {
                    var columns = ConditionTable.Columns;
                    var rows = ConditionTable.Rows[0];
                    foreach (var param in Setting.QueryParameters)
                        if (columns.Contains(param.Key))
                            rows[param.Key] = param.Value;
                }

                if (Setting?.ConditionRequiredColumns != null)
                    SetBooleanProperty(ConditionTable, Setting.ConditionRequiredColumns, MemoryData.ExtendedPropertyNames.IsRequired);
            }

            // 生成汇总数据表。
            if (Setting?.TotalSettings != null)
            {
                var totalTable = new DataTable();
                DataColumn column;
                foreach (var setting in Setting.TotalSettings)
                {
                    if (setting.NumberType == NumberType.Number)
                        column = new DataColumn($"{setting.TableName}_{setting.Name}", typeof(decimal)) { DefaultValue = 0M };
                    else
                        column = new DataColumn($"{setting.TableName}_{setting.Name}", typeof(string)) { DefaultValue = string.Empty };

                    column.Caption = setting.Caption;
                    if (!string.IsNullOrWhiteSpace(setting.Format))
                        column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Format] = setting.Format;
                    totalTable.Columns.Add(column);
                }
                totalTable.Rows.Add(totalTable.NewRow());
                TotalTable = totalTable;
            }

            Compile(basePath, Scripts.ScriptTemplates.CalculatorTemplate, Scripts.ScriptTemplates.CalculatorMethodTemplate, "Compete.Scripts.Calculator", Setting?.ColumnCalculateScriptFileNames, calculatorMethodDictionary);
            Compile(basePath, Scripts.ScriptTemplates.VerifyTemplate, Scripts.ScriptTemplates.VerifyMethodTemplate, "Compete.Scripts.Verifier", Setting?.ColumnVerifyScriptFileNames, verifierMethodDictionary);

            if (Setting?.VerifyScriptFileName != null)
            {
                var assemblyFileName = Scripts.ScriptBuilder.GetAssemblyFileName(basePath, Scripts.ScriptTemplates.VerifyMethodClassName);
                bool isCompile = false;
                var scriptFileName = Path.Combine(basePath, Setting.VerifyScriptFileName);
                if (File.Exists(assemblyFileName))
                {
                    if (File.Exists(scriptFileName)
                        && (new FileInfo(assemblyFileName)).LastWriteTime < (new FileInfo(Path.Combine(basePath, Setting.VerifyScriptFileName)).LastWriteTime))
                        isCompile = true;
                }
                else if (File.Exists(scriptFileName))
                    isCompile = true;

                Type? type = null;
                if (isCompile)
                {
                    var language = Path.GetExtension(scriptFileName).Replace(".", string.Empty);
                    if (string.IsNullOrWhiteSpace(language))
                        language = "cs";
                    type = Scripts.ScriptBuilder.GetType(basePath, Scripts.ScriptTemplates.VerifyethodTemplate, File.ReadAllText(scriptFileName), Scripts.ScriptTemplates.VerifyMethodClassName, language);
                }
                else if (File.Exists(assemblyFileName))
                    type = Assembly.Load(File.ReadAllBytes(assemblyFileName)).GetType(Scripts.ScriptTemplates.VerifyMethodClassName);

                if (type != null)
                    VerifyMethod = type.GetMethod("Verify");
            }

            return base.Initializing();
        }

        private static void Compile(string basePath, string template, string methodTemplate, string className, IDictionary<string, string>? scriptFileNames, Dictionary<string, MethodInfo> methodDictionary, string language = "CSharp")
        {
            if (scriptFileNames == null)
                return;

            string path;
            var assemblyFileName = Scripts.ScriptBuilder.GetAssemblyFileName(basePath, className);
            bool isCompile = false;
            if (File.Exists(assemblyFileName))
            {
                var lastWriteTime = (new FileInfo(assemblyFileName)).LastWriteTime;
                foreach (var fileName in scriptFileNames.Values.Distinct())
                {
                    path = Path.Combine(basePath, fileName);
                    if (File.Exists(path) && lastWriteTime < (new FileInfo(path)).LastWriteTime)
                    {
                        isCompile = true;
                        break;
                    }
                }
            }
            else
                isCompile = true;

            Type? type = null;
            Dictionary<string, string> nameDictionary = [];
            if (isCompile)
            {
                string methodName;
                var methods = new List<string>();
                var scriptBuilder = new StringBuilder();
                foreach (var tablePair in scriptFileNames)
                {
                    path = Path.Combine(basePath, tablePair.Value);
                    methodName = Path.GetFileNameWithoutExtension(path);
                    if (methods.IndexOf(methodName) >= 0)
                        nameDictionary.Add(tablePair.Key, methodName);
                    else if (File.Exists(path))
                    {
                        scriptBuilder.AppendLine(string.Format(methodTemplate, methodName, File.ReadAllText(path)));
                        nameDictionary.Add(tablePair.Key, methodName);
                        methods.Add(methodName);
                    }
                }

                if (nameDictionary.Count > 0)
                    type = Scripts.ScriptBuilder.GetType(basePath, template, scriptBuilder.ToString(), className, language);
            }
            else
            {
                foreach (var tablePair in scriptFileNames)
                {
                    path = Path.Combine(basePath, tablePair.Value);
                    if (File.Exists(path))
                        nameDictionary.Add(tablePair.Key, Path.GetFileNameWithoutExtension(path));
                }

                type = Assembly.Load(File.ReadAllBytes(assemblyFileName)).GetType(className);
            }

            if (type != null)
            {
                MethodInfo? methodInfo;
                foreach (var tablePair in nameDictionary)
                {
                    methodInfo = type.GetMethod(tablePair.Value);
                    if (methodInfo != null)
                        methodDictionary.Add(tablePair.Key, methodInfo);
                }
            }
        }

        private static void Compile(string basePath, string template, string methodTemplate, string className, IDictionary<string, IDictionary<string, string>>? scriptFileNames, Dictionary<string, Dictionary<string, MethodInfo>> methodDictionary, string language = "CSharp")
        {
            if (scriptFileNames == null)
                return;

            string path;
            var assemblyFileName = Scripts.ScriptBuilder.GetAssemblyFileName(basePath, className);
            bool isCompile = false;
            if (File.Exists(assemblyFileName))
            {
                var lastWriteTime = (new FileInfo(assemblyFileName)).LastWriteTime;
                foreach (var dic in scriptFileNames.Values)
                {
                    foreach (var fileName in dic.Values.Distinct())
                    {
                        path = Path.Combine(basePath, fileName);
                        if (File.Exists(path) && lastWriteTime < (new FileInfo(path)).LastWriteTime)
                        {
                            isCompile = true;
                            break;
                        }
                    }
                    if (isCompile)
                        break;
                }
            }
            else
                isCompile = true;

            Dictionary<string, string> tableDictionary;
            Type? type = null;
            Dictionary<string, Dictionary<string, string>> nameDictionary = [];
            if (isCompile)
            {
                string methodName;
                var methods = new List<string>();
                var scriptBuilder = new StringBuilder();
                foreach (var pair in scriptFileNames)
                {
                    tableDictionary = [];
                    foreach (var tablePair in pair.Value)
                    {
                        path = Path.Combine(basePath, tablePair.Value);
                        methodName = Path.GetFileNameWithoutExtension(path);
                        if (methods.IndexOf(methodName) >= 0)
                            tableDictionary.Add(tablePair.Key, methodName);
                        else if (File.Exists(path))
                        {
                            scriptBuilder.AppendLine(string.Format(methodTemplate, methodName, File.ReadAllText(path)));
                            tableDictionary.Add(tablePair.Key, methodName);
                            methods.Add(methodName);
                        }
                    }
                    nameDictionary.Add(pair.Key, tableDictionary);
                }

                if (nameDictionary.Count > 0)
                    type = Scripts.ScriptBuilder.GetType(basePath, template, scriptBuilder.ToString(), className, language);
            }
            else
            {
                foreach (var pair in scriptFileNames)
                {
                    tableDictionary = [];
                    foreach (var tablePair in pair.Value)
                    {
                        path = Path.Combine(basePath, tablePair.Value);
                        if (File.Exists(path))
                            tableDictionary.Add(tablePair.Key, Path.GetFileNameWithoutExtension(path));
                    }
                    nameDictionary.Add(pair.Key, tableDictionary);
                }

                type = Assembly.Load(File.ReadAllBytes(assemblyFileName)).GetType(className);
            }

            if (type != null)
            {
                MethodInfo? methodInfo;
                Dictionary<string, MethodInfo> tableMethodDictionary;
                foreach (var pair in nameDictionary)
                {
                    tableMethodDictionary = [];
                    foreach (var tablePair in pair.Value)
                    {
                        methodInfo = type.GetMethod(tablePair.Value);
                        if (methodInfo != null)
                            tableMethodDictionary.Add(tablePair.Key, methodInfo);
                    }
                    methodDictionary.Add(pair.Key, tableMethodDictionary);
                }
            }
        }

        private readonly Dictionary<DataColumn, MethodInfo> conditionColumnCalculators = [];

        private readonly Dictionary<string, MethodInfo> conditionCalculatorMethodDictionary = [];

        private readonly Dictionary<DataColumn, MethodInfo> conditionColumnVerifiers = [];

        private readonly Dictionary<string, MethodInfo> conditionVerifierMethodDictionary = [];

        private readonly Dictionary<DataColumn, MethodInfo> columnCalculators = [];

        private readonly Dictionary<string, Dictionary<string, MethodInfo>> calculatorMethodDictionary = [];

        private readonly Dictionary<DataColumn, MethodInfo> columnVerifiers = [];

        private readonly Dictionary<string, Dictionary<string, MethodInfo>> verifierMethodDictionary = [];

        private MethodInfo? VerifyMethod;

        private static void SetBooleanProperty(DataTable table, IEnumerable<string> columns, MemoryData.ExtendedPropertyNames propertyName, bool propertyValue = true)
        {
            foreach (var name in columns)
                if (table.Columns.Contains(name))
                    table.Columns[name]!.ExtendedProperties[propertyName] = propertyValue;
        }

        private static void SetBooleanProperty(DataSet data, IDictionary<string, IEnumerable<string>>? columns, MemoryData.ExtendedPropertyNames propertyName, bool propertyValue = true)
        {
            if (columns == null)
                return;

            foreach (var pair in columns)
                if (data.Tables.Contains(pair.Key))
                    SetBooleanProperty(data.Tables[pair.Key]!, pair.Value, propertyName, propertyValue);
        }

        protected virtual void ProcessData(DataSet data) { }

        protected override bool CanQuery() => (!IsInitializing || Setting!.IsInitialQuery) && base.CanQuery();

        protected override void QueryData(string? name)
        {
            var mainData = Setting?.MemoryDataSettings == null ? null : MemoryData.DataCreator.Create(Setting.MemoryDataSettings);

            if (Setting?.MemoryData != null)
            {
                var data = MemoryData.DataCreator.Create(Setting.MemoryData);
                if (mainData == null)
                    mainData = data;
                else
                    mainData.Merge(data);
            }

            if (!string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(Setting!.DataLoadName))
            {
                IDictionary<string, object>? parameters = ConditionTable == null || ConditionTable.Rows.Count == 0 ? null : ConditionTable.Rows[0].ToDictionary(Setting!.QueryConditionColumns);

                if (Setting!.QueryParameters != null)
                {
                    var queryParameters = Setting.QueryParameters.ClearNull();
                    if (queryParameters.Count > 0)
                        if (parameters == null)
                            parameters = queryParameters;
                        else
                            parameters.Merge(queryParameters);
                }

                if (parameters != null && Setting.QueryConditionExcludedColumns != null)
                    foreach (var column in Setting.QueryConditionExcludedColumns)
                        parameters.Remove(column);

                // 清除空实体
                if (parameters != null)
                    foreach(var pair in parameters)
                        if (pair.Key.EndsWith("_Id", StringComparison.OrdinalIgnoreCase) && long.TryParse(pair.Value.ToString(), out long value) && value == 0L)
                            parameters.Remove(pair.Key);

                if (PluginParameter?.Data != null)
                    if (parameters == null)
                        parameters = PluginParameter?.Data;
                    else
                        parameters.Merge(PluginParameter.Data);

                DataSet data;
                if (Setting.IsPagingQuery)
                {
                    var result = GlobalCommon.DataProvider!.PagingQuery(PluginParameter!.Path, (name ?? Setting.DataLoadName)!, parameters, CurrentPageNo, PageSize);
                    data = result.Data!;
                    RecordCount = result.Count;
                    CurrentPageNo = result.PageNo;
                }
                else
                    data = GlobalCommon.DataProvider!.Query(PluginParameter!.Path, (name ?? Setting.DataLoadName)!, parameters);

                if (mainData == null)
                    mainData = data;
                else
                    mainData.Merge(data);
            }

            if (mainData != null)
            {
                ProcessData(mainData);

                if (Setting?.AdditionalDataColumns != null)
                    MemoryData.DataCreator.AddColumns(mainData, Setting.AdditionalDataColumns);

                if (Setting?.AdditionalDataColumnSettings != null)
                    MemoryData.DataCreator.AddColumns(mainData, Setting.AdditionalDataColumnSettings);

                if (Setting?.DataColumnSettings != null)
                    MemoryData.DataCreator.SetColumns(mainData, Setting.DataColumnSettings);

                if (Setting?.ColumnDisplayIndexes != null)
                {
                    DataTable table;
                    foreach (var pair in Setting.ColumnDisplayIndexes)
                        if (mainData.Tables.Contains(pair.Key))
                        {
                            table = mainData.Tables[pair.Key]!;
                            foreach (var tablePair in pair.Value)
                                if (table.Columns.Contains(tablePair.Key))
                                    table.Columns[tablePair.Key]!.ExtendedProperties[MemoryData.ExtendedPropertyNames.DisplayIndex] = tablePair.Value;
                        }
                }

                SetBooleanProperty(mainData, Setting?.InvisibleColumns, MemoryData.ExtendedPropertyNames.IsVisible, false);
                SetBooleanProperty(mainData, Setting?.ReadOnlyColumns, MemoryData.ExtendedPropertyNames.IsReadOnly);
                SetBooleanProperty(mainData, Setting?.RequiredColumns, MemoryData.ExtendedPropertyNames.IsRequired);

                if (Setting?.EditableColumns != null)
                {
                    DataColumnCollection columns;
                    bool isReadOnly;
                    foreach (var pair in Setting.EditableColumns)
                        if (mainData.Tables.Contains(pair.Key))
                        {
                            columns = mainData.Tables[pair.Key]!.Columns;
                            foreach (DataColumn column in mainData.Tables[pair.Key]!.Columns)
                            {
                                isReadOnly = true;
                                foreach (var columnName in pair.Value)
                                    if (column.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsReadOnly] = false;
                                        isReadOnly = false;
                                        break;
                                    }
                                if (isReadOnly)
                                    column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsReadOnly] = true;
                            }
                        }
                }

                if (Setting?.InvisibleIdTables != null)
                    foreach(var tableNname in Setting.InvisibleIdTables)
                        foreach (DataColumn column in mainData.Tables[tableNname]!.Columns)
                            if (column.ColumnName.EndsWith("_Id", StringComparison.OrdinalIgnoreCase))
                                column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsVisible]
                                    = (column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsVisible] as bool? ?? true)
                                    && !(bool)column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsReadOnly]!;

                if (mainData.Tables.Count > 0)
                {
                    if (Setting!.IsAddMaster && mainData.Tables[0].Rows.Count == 0)
                        mainData.Tables[0].Rows.Add(mainData.Tables[0].NewRow());

                    if (calculatorMethodDictionary.Count > 0)
                    {
                        columnCalculators.Clear();
                        foreach (DataTable table in mainData.Tables)
                            if (calculatorMethodDictionary.TryGetValue(table.TableName, out Dictionary<string, MethodInfo>? methodDictionary))
                            {
                                foreach (DataColumn column in table.Columns)
                                    if (methodDictionary.TryGetValue(column.ColumnName, out MethodInfo? method))
                                        columnCalculators.Add(column, method);
                                table.ColumnChanged += Calculator_Table_ColumnChanged;
                            }
                    }

                    if (verifierMethodDictionary.Count > 0)
                    {
                        columnVerifiers.Clear();
                        foreach (DataTable table in mainData.Tables)
                            if (verifierMethodDictionary.TryGetValue(table.TableName, out Dictionary<string, MethodInfo>? methodDictionary))
                            {
                                foreach (DataColumn column in table.Columns)
                                    if (methodDictionary.TryGetValue(column.ColumnName, out MethodInfo? method))
                                        columnVerifiers.Add(column, method);
                                table.ColumnChanging += Verifier_Table_ColumnChanging; ;
                            }
                    }

                    if (Setting?.TotalSettings != null)
                        foreach (DataTable table in mainData.Tables)
                        {
                            table.RowChanged += Table_RowChanged_Total;
                            table.RowDeleted += Table_RowDeleted_Total;
                        }
                }
            }

            Data = mainData;
        }

        protected override void ConditionTableChanged(DataTable? oldValue, DataTable? newValue)
        {
            if (oldValue != null)
            {
                oldValue.ColumnChanging -= ConditionTable_ColumnChanging;
                oldValue.ColumnChanged -= ConditionTable_ColumnChanged;
            }

            if (newValue != null)
            {
                var columns = newValue.Columns;

                if (conditionVerifierMethodDictionary.Count > 0)
                {
                    conditionColumnVerifiers.Clear();
                    foreach (DataColumn column in columns)
                        if (conditionVerifierMethodDictionary.TryGetValue(column.ColumnName, out MethodInfo? method))
                            conditionColumnVerifiers.Add(column, method);

                    newValue.ColumnChanging += ConditionTable_ColumnChanging;
                }

                if (conditionCalculatorMethodDictionary.Count > 0)
                {
                    conditionColumnCalculators.Clear();
                    foreach (DataColumn column in columns)
                        if (conditionCalculatorMethodDictionary.TryGetValue(column.ColumnName, out MethodInfo? method))
                            conditionColumnCalculators.Add(column, method);

                    newValue.ColumnChanged += ConditionTable_ColumnChanged;
                }
            }

            base.ConditionTableChanged(oldValue, newValue);
        }

        private static void ExecuteVerifier(DataColumnChangeEventArgs e, Dictionary<DataColumn, MethodInfo> methodDictionary)
        {
            if (e.Column == null || !methodDictionary.TryGetValue(e.Column, out MethodInfo? verifier))
                return;

            var message = verifier.Invoke(null, [e.Row, e.ProposedValue])?.ToString();
            if (!string.IsNullOrWhiteSpace(message))
            {
                MisControls.MessageDialog.Warning(message);
                e.ProposedValue = e.Row[e.Column];
                e.Row.CancelEdit();
            }
        }

        private void ExecuteCalculator(DataColumnChangeEventArgs e, Dictionary<DataColumn, MethodInfo> methodDictionary)
        {
            if (isEditting || e.Column == null || !methodDictionary.TryGetValue(e.Column, out MethodInfo? calculator))
                return;

            isEditting = true;

            calculator.Invoke(null, [e.Row]);

            isEditting = false;
        }

        private void ConditionTable_ColumnChanging(object sender, DataColumnChangeEventArgs e) => ExecuteVerifier(e, conditionColumnVerifiers);

        private void ConditionTable_ColumnChanged(object sender, DataColumnChangeEventArgs e) => ExecuteCalculator(e, conditionColumnCalculators);

        private void Verifier_Table_ColumnChanging(object sender, DataColumnChangeEventArgs e) => ExecuteVerifier(e, columnVerifiers);

        private bool isEditting = false;

        private void Calculator_Table_ColumnChanged(object sender, DataColumnChangeEventArgs e) => ExecuteCalculator(e, columnCalculators);

        private void ComputeTotal(DataTable table)
        {
            var settings = from setting in Setting!.TotalSettings
                           where setting.TableName == table.TableName
                           select setting;

            object totalValue;
            decimal total;
            foreach (var setting in settings)
            {
                totalValue = table.Compute(string.IsNullOrWhiteSpace(setting.Expression) ? $"SUM({setting.Name})" : setting.Expression, null);
                if (totalValue == DBNull.Value)
                    continue;

                total = Convert.ToDecimal(totalValue);
                TotalTable!.Rows[0][$"{setting.TableName}_{setting.Name}"]
                    = setting.NumberType switch
                    {
                        NumberType.WordNumber => Utils.Chinese.ConvertNumerals(total),
                        NumberType.WordCurrency => Utils.Chinese.ConvertAmount(total),
                        _ => total,
                    };
            }
        }

        private void Table_RowChanged_Total(object sender, DataRowChangeEventArgs e) => ComputeTotal(e.Row.Table);

        private void Table_RowDeleted_Total(object sender, DataRowChangeEventArgs e) => ComputeTotal(e.Row!.Table!);

        protected override void OnQueried(EventArgs e)
        {
            base.OnQueried(e);

            if (Setting!.IsAddMaster && MasterData?.Count == 0)
                MasterData.AddNew();

            if (TotalTable != null)
            {
                foreach (DataColumn column in TotalTable.Columns)
                    TotalTable.Rows[0][column] = column.DefaultValue;

                foreach (DataTable table in Data!.Tables)
                    ComputeTotal(table);
            }
        }

        private static DataTable? RemoveColumns(DataView dataView, DataTable? table, string[]? columns, DataViewRowState state)
        {
            if (columns == null)
                return table?.Rows.Count > 0 ? table : null;

            if (table == null)
            {
                dataView.RowStateFilter = state;
                table = dataView.ToTable();
            }

            foreach (var column in columns)
                table.Columns.Remove(column);

            return table.Rows.Count > 0 ? table : null;
        }

        private static DataTable RemoveColumns(DataTable table, string[]? columns) => columns == null || columns.LongLength == 0L ? table.Copy() : table.DefaultView.ToTable(false, columns);

        private static DataTable? GetTable(DataView dataView, string[]? columns, DataViewRowState state)
        {
            //if (columns == null)
            //    return null;

            //dataView.RowStateFilter = state;
            //var result = columns.LongLength == 1L && columns[0] == "*"
            //    ? dataView.ToTable()
            //    : dataView.ToTable(true, columns);
            //return result.Rows.Count == 0 ? null : result;

            dataView.RowStateFilter = state;
            var result = columns == null || columns.LongLength == 1L && columns[0] == "*"
                ? dataView.ToTable()
                : dataView.ToTable(true, columns);
            return result.Rows.Count == 0 ? null : result;
        }

        protected override bool SaveData(string? name)
        {
            Common.Result result;

            DataView dataView;
            if (Setting!.IsDifferentiated)
            {
                SplitData? tableData;
                var data = new Dictionary<string, SplitData>();

                if (Setting.DifferentiatedSaveColumns != null)
                    foreach (var pair in Setting.DifferentiatedSaveColumns)
                    {
                        tableData = new SplitData();
                        dataView = new DataView(Data!.Tables[pair.Key]);

                        tableData.AddedTable = GetTable(dataView, pair.Value.AddedColumns, DataViewRowState.Added);
                        tableData.DeletedTable = GetTable(dataView, pair.Value.DeletedColumns, DataViewRowState.Deleted);
                        tableData.ModifiedTable = GetTable(dataView, pair.Value.ModifiedColumns, DataViewRowState.ModifiedCurrent);
                        tableData.ModifiedOriginalTable = GetTable(dataView, pair.Value.ModifiedOriginalColumns, DataViewRowState.ModifiedOriginal);

                        //if (tableData.AddedTable != null || tableData.DeletedTable != null || tableData.ModifiedTable != null || tableData.ModifiedOriginalTable != null)
                        data.Add(pair.Key, tableData);
                    }

                if (Setting.DifferentiatedSaveRemoveColumns != null)
                    foreach(var pair in Setting.DifferentiatedSaveRemoveColumns)
                    {
                        dataView = new DataView(Data!.Tables[pair.Key]);
                        if (!data.TryGetValue(pair.Key, out tableData))
                        {
                            tableData = new SplitData();
                            data.Add(pair.Key, tableData);
                        }

                        if (Setting.SaveFilters != null && Setting.SaveFilters!.TryGetValue(pair.Key, out var filter) && !string.IsNullOrWhiteSpace(filter))
                            dataView.RowFilter = filter;

                        tableData.AddedTable = RemoveColumns(dataView, tableData.AddedTable, pair.Value.AddedColumns, DataViewRowState.Added);
                        tableData.DeletedTable = RemoveColumns(dataView, tableData.DeletedTable, pair.Value.DeletedColumns, DataViewRowState.Deleted);
                        tableData.ModifiedTable = RemoveColumns(dataView, tableData.ModifiedTable, pair.Value.ModifiedColumns, DataViewRowState.ModifiedCurrent);
                        tableData.ModifiedOriginalTable = RemoveColumns(dataView, tableData.ModifiedOriginalTable, pair.Value.ModifiedOriginalColumns, DataViewRowState.ModifiedOriginal);
                    }

                result = GlobalCommon.DataProvider!.Save(PluginParameter!.Path, name ?? Setting?.DataSaveName ?? "save", data, ActionId!.Value);
            }
            else
            {
                var dataSet = new DataSet();
                if (Setting.IsMergeConditionData && ConditionTable != null)
                    dataSet.Tables.Add(RemoveColumns(ConditionTable, Setting.SaveConditionColumns));

                foreach (DataTable table in Data!.Tables)
                {
                    dataView = new DataView(table)
                    {
                        RowStateFilter = Setting.SaveRowStateFilters == null || !Setting.SaveRowStateFilters!.TryGetValue(table.TableName, out var rowStateFilter)
                            ? DataViewRowState.Added | DataViewRowState.ModifiedCurrent
                            : rowStateFilter,
                    };
                    
                    if (Setting.SaveFilters != null && Setting.SaveFilters!.TryGetValue(table.TableName, out var filter) && !string.IsNullOrWhiteSpace(filter))
                        dataView.RowFilter = filter;

                    if (Setting.SaveColumns != null && Setting.SaveColumns!.TryGetValue(table.TableName, out var columns))
                        dataSet.Tables.Add(dataView.ToTable(false, columns));
                    else
                        dataSet.Tables.Add(dataView.ToTable());
                }
                result = GlobalCommon.DataProvider!.Save(PluginParameter!.Path, name ?? Setting?.DataSaveName ?? "save", dataSet, ActionId!.Value);
            }

            if (result.ErrorNo < 0)
            {
                ShowMessage(result);

                return true;
            }

            return base.SaveData(name);
        }

        protected override bool Verify()
        {
            if (Data == null || Data.Tables.Count == 0 || Setting!.IsMergeConditionData && !VerifyConditionTable())
                return false;

            var hasError = false;
            var builder = new StringBuilder();
            if (Setting.RequiredTables != null)
                foreach (var pair in Setting.RequiredTables)
                    if ((!Data.Tables.Contains(pair.Key) || !(from row in Data.Tables[pair.Key]!.AsEnumerable()
                                                                where row.RowState != DataRowState.Unchanged
                                                                select row).Any()) && !string.IsNullOrWhiteSpace(pair.Value))
                    {
                        builder.Append(pair.Value);
                        builder.Append('\t');
                        hasError = true;
                        //if (string.IsNullOrWhiteSpace(MessageText))
                        //    MessageText = pair.Value;
                        //else
                        //    MessageText += pair.Value;
                        //return false;
                    }

            if (VerifyMethod != null)
            {
                var message = VerifyMethod.Invoke(null, [Data!])?.ToString();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    builder.Append(GlobalCommon.GetMessage(message));
                    builder.Append('\t');
                    hasError = true;
                    //MessageText = GlobalCommon.GetMessage(message);
                    //return false;
                }
            }

            var messageText = builder.ToString();
            if (hasError)
            {
                MessageText += messageText;
                return false;
            }

            return true;
        }

        protected override bool ExecuteSaveSql(string name)
        {
            var dataSet = new DataSet();
            if (Setting!.IsMergeConditionData && ConditionTable != null)
                dataSet.Tables.Add(RemoveColumns(ConditionTable, Setting.SaveConditionColumns));

            var result = GlobalCommon.DataProvider!.Save(PluginParameter!.Path, name, dataSet, ActionId!.Value);    // Setting?.DataSaveName ?? 

            if (result.ErrorNo < 0)
            {
                ShowMessage(result);

                return true;
            }

            return base.ExecuteSaveSql(name);
        }

        protected override bool CanDirectExecuteSql => Setting?.IsMergeConditionData == true ? VerifyConditionTable() : base.CanDirectExecuteSql;


        private void ShowMessage(Common.Result result)
        {
            if (string.IsNullOrWhiteSpace(result.Message))
            {
                if (Setting?.ErrorDictionary != null && Setting.ErrorDictionary.TryGetValue(result.ErrorNo, out string? message))
                    MessageText = message;
                else if (GlobalCommon.ErrorDictionary != null && GlobalCommon.ErrorDictionary.TryGetValue(result.ErrorNo, out string? globalMessage))
                    MessageText = globalMessage;
            }
            else
                MessageText = result.Message;

            MisControls.MessageDialog.Warning(MessageText ?? string.Empty);
        }

        protected override FlowDocument? GetDocument()
        {
            if (string.IsNullOrWhiteSpace(Setting?.PrintDocumentFileName))
                return null;

            var path = Path.Combine(GlobalCommon.PluginPath, PluginParameter!.Path, Setting.PrintDocumentFileName);
            return File.Exists(path) ? (FlowDocument)XamlReader.Parse(File.ReadAllText(path)) : null;
        }
        //=> string.IsNullOrWhiteSpace(Setting?.PrintDocumentFileName) || !File.Exists(Path.Combine(GlobalCommon.PluginPath, PluginParameter!.Path, Setting.PrintDocumentFileName))
        //? null
        //: (FlowDocument)XamlReader.Parse(File.ReadAllText(Setting.PrintDocumentFileName));
        //{
        //    // => (FlowDocument)XamlReader.Parse(File.ReadAllText("D:/Projects/CompeteMIS/tmp/TestFlowDocument.xaml"));
        //    //var doc = (FlowDocument)XamlReader.Parse(File.ReadAllText("D:/Projects/CompeteMIS/tmp/TestFlowDocument.xaml"));
        //    ////var paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
        //    ////paginator.PageSize = new Size(595, 842);
        //    ////doc.ColumnWidth = double.PositiveInfinity;

        //    //var window = new Print.PrintPreviewWindow();

        //    ////Dispatcher.CurrentDispatcher.BeginInvoke(new PrintHelper.LoadXpsMethod(PrintHelper.LoadXps), DispatcherPriority.ApplicationIdle, window.documentViewer, doc);
        //    //Dispatcher.CurrentDispatcher.BeginInvoke(MisControls.PrintHelper.LoadXps, DispatcherPriority.ApplicationIdle, window.documentViewer, doc);
        //    ////PrintHelper.LoadXps(doc, window.docViewer);

        //    //window.Show();

        //    return Setting?.PrintDocumentFileName == null || !File.Exists(Setting.PrintDocumentFileName) ? null :(FlowDocument) XamlReader.Parse(File.ReadAllText(Setting.PrintDocumentFileName));
        //}


        protected override PluginCommandParameter GetRunParameter(PluginCommandParameter parameter)
        {
            var currentItem = MasterData?.CurrentItem as DataRowView;
            if (Setting?.RunColumns == null)
                parameter.Data = currentItem?.Row.ToDictionary();
            else
            {
                var dataParameters = new Dictionary<string, object>();
                foreach (var column in Setting.RunColumns)
                    dataParameters.Add(column, currentItem![column]);
                parameter.Data = dataParameters;
            }

            return base.GetRunParameter(parameter);
        }
    }
}
