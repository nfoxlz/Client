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
        /// 执行其它插件。2的0次幂。
        /// </summary>
        Run = 1L,

        /// <summary>
        /// 执行脚本。2的1次幂。
        /// </summary>
        ExecuteScript = 2L,

        /// <summary>
        /// 查询。2的2次幂。
        /// </summary>
        Query = 4L,

        /// <summary>
        /// 打印。2的3次幂。
        /// </summary>
        Print = 8L,

        /// <summary>
        /// 增加。2的4次幂。
        /// </summary>
        Add = 16L,

        /// <summary>
        /// 删除。2的5次幂。
        /// </summary>
        Delete = 32L,

        /// <summary>
        /// 修改。2的6次幂。
        /// </summary>
        Modify = 64L,

        /// <summary>
        /// 保存。2的7次幂。
        /// </summary>
        Save = Add | Delete | Modify,

        /// <summary>
        /// 审核。2的8次幂。
        /// </summary>
        Audit = 128L,

        /// <summary>
        /// 记账。2的9次幂。
        /// </summary>
        Keep = 256L,

        /// <summary>
        /// 直接保存。2的10次幂。
        /// </summary>
        DirectSave = 512L,

        /// <summary>
        /// 直接增加。2的11次幂。
        /// </summary>
        DirectAdd = 1024L,

        /// <summary>
        /// 直接删除。2的12次幂。
        /// </summary>
        DirectDelete = 2048L,

        /// <summary>
        /// 直接修改。2的13次幂。
        /// </summary>
        DirectModify = 4096L,

        /// <summary>
        /// 批量删除。2的14次幂。
        /// </summary>
        BatchDelete = 8192L,

        /// <summary>
        /// 批量审核。2的15次幂。
        /// </summary>
        BatchAudit = 16384L,

        /// <summary>
        /// 批量记账。2的16次幂。
        /// </summary>
        BatchKeep = 32768L,

        /// <summary>
        /// 执行SQL。2的17次幂。
        /// </summary>
        ExecuteSql = 65536L,

        /// <summary>
        /// 扩展保存。2的18次幂。
        /// </summary>
        ExtendedSave = 131072L,

        /// <summary>
        /// 增加子项。2的19次幂。
        /// </summary>
        AddChild = 262144L,

        /// <summary>
        /// 直接增加子项。2的20次幂。
        /// </summary>
        DirectAddChild = 524288L,

        /// <summary>
        /// 删除子项。2的21次幂。
        /// </summary>
        DeleteChild = 1048576L,

        /// <summary>
        /// 直接删除子项。2的22次幂。
        /// </summary>
        DirectDeleteChild = 2097152L,

        /// <summary>
        /// 查询返回消息。2的23次幂。
        /// </summary>
        QueryMessage = 4194304L,
    }
}
