namespace Compete.Mis.Models
{
    public sealed class SimpleData
    {
        public string? TableName { get; set; }

        public string[]? Columns { get; set; }

        public object?[][]? Rows { get; set; }
    }
}
