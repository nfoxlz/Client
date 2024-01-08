namespace Compete.Mis.Models
{
    public sealed class SimpleDataTable
    {
        public string? TableName { get;set; }

        public string[]? Columns { get; set; }

        public object?[][]? Rows { get; set; }
    }
}
