using System;

namespace Compete.MemoryData
{
    public sealed record DataColumnSetting : DataColumnExtendedSetting
    {
        public required string ColumnName { get; set; }

        public string? Caption { get; set; }

        public string? Comment { get; set; }

        public Type? DataType { get; set; }

        public int MaxLength { get; set; } = -1;

        public bool AllowDBNull { get; set; } = true;

        public bool ReadOnly { get; set; }

        public object? DefaultValue { get; set; }

        public bool IsUnique { get; set; }

        public bool IsAutoIncrement { get; set; }

        public long AutoIncrementSeed { get; set; } = 1L;

        public long AutoIncrementStep { get; set; } = 1L;

        public string? Expression { get; set; }

        public int DisplayIndex { get; set; } = -1;
    }
}
