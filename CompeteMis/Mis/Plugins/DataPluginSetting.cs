﻿using System.Collections.Generic;
using System.Data;

namespace Compete.Mis.Plugins
{
    public class DataPluginSetting : UIPluginSetting
    {
        public string? Title { get; set; }

        public IEnumerable<string>? ConditionMemoryDataSettings { get; set; }

        public Models.SimpleData? ConditionMemoryData { get; set; }

        public IEnumerable<MemoryData.DataColumnSetting>? ConditionDataColumnSettings { get; set; }

        public string[]? ConditionEditableColumns { get; set; }

        public string[]? QueryConditionColumns { get; set; }

        public string[]? QueryConditionExcludedColumns { get; set; }

        public string? DataLoadName { get; set; }

        public bool IsPagingQuery { get; set; }

        public IDictionary<string, IEnumerable<string>>? MemoryDataSettings { get; set; }

        //public string? MemoryDataName { get; set; }

        public IList<Models.SimpleData>? MemoryData { get; set; }

        public bool IsMergePluginParameter { get; set; }

        //public string? DataSettingsName { get; set; }

        public IDictionary<string, IEnumerable<string>>? AdditionalDataColumns { get; set; }

        public IDictionary<string, IEnumerable<MemoryData.DataColumnSetting>>? AdditionalDataColumnSettings { get; set; }

        public IDictionary<string, IDictionary<string, int>>? ColumnDisplayIndexes { get; set; }

        //public string? DataColumnSettingsName { get; set; }

        public IDictionary<string, IEnumerable<MemoryData.DataColumnSetting>>? DataColumnSettings { get; set; }

        public bool IsInitialQuery { get; set; } = true;

        public bool IsAddMaster { get; set; } = true;

        public IEnumerable<string>? ConditionRequiredColumns { get; set; }

        public IDictionary<string, IEnumerable<string>>? InvisibleColumns { get; set; }

        public IDictionary<string, IEnumerable<string>>? ReadOnlyColumns { get; set; }

        public IDictionary<string, IEnumerable<string>>? EditableColumns { get; set; }

        public IDictionary<string, IEnumerable<string>>? RequiredColumns { get; set; }

        public IDictionary<string, IEnumerable<string>>? UniqueColumns { get; set; }

        public IDictionary<string, IEnumerable<string>>? NewCopyColumns { get; set; }

        public IDictionary<string, IEnumerable<string>>? ZeroIsEmptyColumns { get; set; }

        public IEnumerable<string>? InvisibleIdTables { get; set; }

        public IDictionary<string, object?>? QueryParameters { get; set; }

        public IEnumerable<TotalSetting>? TotalSettings { get; set; }

        public string? InitializingScriptFileName { get; set; }

        public string? InitializedScriptFileName { get; set; }

        public IDictionary<string, string>? ConditionCalculateScriptFileNames { get; set; }

        public IDictionary<string, string>? ConditionVerifyScriptFileNames { get; set; }

        public IDictionary<string, IDictionary<string, string>>? ColumnCalculateScriptFileNames { get; set; }

        public IDictionary<string, IDictionary<string, string>>? ColumnVerifyScriptFileNames { get; set; }

        public string? PrintDocumentFileName { get; set; } = "print.xaml";

        public string? DataSaveName { get; set; }

        public bool IsMergeConditionData { get; set; }

        public IDictionary<string, DataViewRowState>? SaveRowStateFilters { get; set; }

        public IDictionary<string, string>? SaveFilters { get; set; }

        public string[]? SaveConditionColumns { get; set; }

        public bool IsMergeConditionTable { get;set; }

        public IDictionary<string, string[]>? SaveColumns { get; set; }

        public IDictionary<string, string>? RequiredTables { get; set; }

        public IDictionary<string, string>? RequiredDataTables { get; set; }

        public string? VerifyScriptFileName { get; set; } = "Verify.cs";

        public string? QueringScriptFileName { get; set; } = "Quering.cs";

        public string? QueriedScriptFileName { get; set; } = "Queried.cs";

        public bool IsDifferentiated { get; set; }

        public IDictionary<string, SaveDataSetting>? DifferentiatedSaveColumns { get; set; }

        public IDictionary<string, SaveDataSetting>? DifferentiatedSaveRemoveColumns { get; set; }

        public IEnumerable<string>? UnmodifiedSaveTables { get; set; }

        public string[]? RunColumns { get; set; }

        public IDictionary<int, string>? ErrorDictionary { get; set; }
    }
}
