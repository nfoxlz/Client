namespace Compete.Mis.Frame.Services.ServiceModels
{
    public sealed class MenuSetting
    {
        /// <summary>
        /// 获取或设置菜单标识。
        /// </summary>
        public long MenuNo { get; set; }

        /// <summary>
        /// 获取或设置菜单父标识。
        /// </summary>
        public long ParentMenuNo { get; set; }

        /// <summary>
        /// 序号。
        /// </summary>
        public long Sn { get; set; }

        /// <summary>
        /// 获取或设置菜单显示名。
        /// </summary>
        public required string DisplayName { get; set; }

        /// <summary>
        /// 获取或设置工具提示。
        /// </summary>
        public string? ToolTip { get; set; }

        /// <summary>
        /// 获取或设置菜单调用功能所在程序集。
        /// </summary>
        public string? PluginSetting { get; set; }

        /// <summary>
        /// 获取或设置参数。
        /// </summary>
        public string? PluginParameter { get; set; }

        /// <summary>
        /// 获取或设置菜单功能的内部权限。
        /// </summary>
        public long Authorition { get; set; }
    }
}
