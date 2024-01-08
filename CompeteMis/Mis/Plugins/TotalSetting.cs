namespace Compete.Mis.Plugins
{
    public sealed class TotalSetting
    {
        /// <summary>
        /// 获取或设置汇总项名称。
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 获取或设置汇总项标题。
        /// </summary>
        public string? Caption { get; set; }

        /// <summary>
        /// 获取或设置一个值，用于指示汇总项所汇总的表名。
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// 获取或设置汇总项的汇总表达式。
        /// </summary>
        public string? Expression { get; set; }

        /// <summary>
        /// 获取或设置汇总项的数字类型。
        /// </summary>
        public NumberType NumberType { get; set; }

        public string? Format { get; set; }
    }
}
