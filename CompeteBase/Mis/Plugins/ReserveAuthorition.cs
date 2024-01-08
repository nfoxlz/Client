using System;

namespace Compete.Mis.Plugins
{
    /// <summary>
    /// 预留权限枚举。
    /// </summary>
    [Flags]
    public enum ReserveAuthorition : long
    {
        /// <summary>
        /// 全部权限。
        /// </summary>
        All = -1L,

        /// <summary>
        /// 执行其它插件。
        /// </summary>
        Run = 1L,

        /// <summary>
        /// 执行脚本。
        /// </summary>
        ExecuteScript = 2L,

        /// <summary>
        /// 查询。
        /// </summary>
        Query = 4L,

        /// <summary>
        /// 打印。
        /// </summary>
        Print = 8L,

        /// <summary>
        /// 增加。
        /// </summary>
        Add = 16L,

        /// <summary>
        /// 删除。
        /// </summary>
        Delete = 32L,

        /// <summary>
        /// 修改。
        /// </summary>
        Modify = 64L,

        /// <summary>
        /// 保存。
        /// </summary>
        Save = Add | Delete | Modify,

        /// <summary>
        /// 审核。
        /// </summary>
        Audit = 128L,

        /// <summary>
        /// 直接增加。
        /// </summary>
        DirectAdd = 256L,

        /// <summary>
        /// 直接删除。
        /// </summary>
        DirectDelete = 512L,

        /// <summary>
        /// 直接修改。
        /// </summary>
        DirectModify = 1024L,

        /// <summary>
        /// 批量删除。
        /// </summary>
        BatchDelete = 2048L,

        /// <summary>
        /// 批量审核。
        /// </summary>
        BatchAudit = 4096L,

        /// <summary>
        /// 执行SQL。
        /// </summary>
        ExecuteSql = 8192L,
    }
}
