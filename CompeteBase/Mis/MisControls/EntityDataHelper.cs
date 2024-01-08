// ======================================================
// XXX项目
// ======================================================
// 版权所有 © Compete software studio 2019 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间            作者     说明
// ------------------------------------------------------
// 1.0.0.0 2019/8/1 周四 下午 3:24:28 LeeZheng 新建。
// ======================================================

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// EntityDataHelper 类。
    /// </summary>
    internal static class EntityDataHelper
    {
        private static readonly string[] columns = ["Bill_Id"];//"Id", 

        public static bool IsEntityColumn(string name)
        {
            foreach (var column in columns)
                if (column == name)
                    return false;
            return true;
        }
    }
}
