using System.Data;

namespace Compete.MemoryData
{
    public record DataColumnExtendedSetting
    {
        public bool IsVisible { get; set; } = true;

        public bool IsReadOnly { get; set; }

        public bool IsRequired { get; set; }

        //public int Length { get; set; }

        public short Precision { get; set; }

        public decimal? MaxValue { get; set; }

        public decimal? MinValue { get; set; }

        public string? Format { get; set; }

        public string? ShowFormat { get; set; }

        public DbType DbDataType { get; set; }

        public string? DbPhysicalType { get; set; }

        public Common.TypeSetting? EditControl { get; set; }

        public SystemVariables DefaultSystemValue { get; set; }

        public object? TargetNullValue { get; set; }

        public string? Regex { get; set; }

        public string? ErrorText { get; set; }

        public Mis.MisControls.DataControlType Control { get; set; }

        public string? Parameters { get; set; }
    }
}
