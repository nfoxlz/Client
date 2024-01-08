// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/4/18 13:20:51 LeeZheng 新建。
//==============================================================
using System.Data;

namespace Compete.Mis.Developer.PowerDesigner
{
    /// <summary>
    /// IDataTypeConverter 接口。
    /// </summary>
    internal interface IDataTypeConverter
    {
        /// <summary>
        /// 由数据库数据类型到DbType的转换。
        /// </summary>
        /// <param name="dataType">数据库数据类型</param>
        /// <returns>DbType类型。</returns>
        DbType DataTypeToDbType(string dataType);

        string DefaultCurrentDateTime { get; }

        string DefaultCurrentDate { get; }
    }
}
