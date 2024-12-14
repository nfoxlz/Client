// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/12 10:39:12 LeeZheng 新建。
//==============================================================
using System.Collections.Generic;
using System.Data;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// 实体数据提供者接口。
    /// </summary>
    public interface IEntityDataProvider
    {
        /// <summary>
        /// 依条件查询数据。
        /// </summary>
        /// <param name="name">标识数据的名称。</param>
        /// <param name="conditions">条件。</param>
        /// <param name="currentPageNo">当前页号。</param>
        /// <param name="pageSize">每页记录数。</param>
        /// <returns>查询到的数据。</returns>
        Models.PagingDataQueryResult Query(string name, IDictionary<string, object>? conditions, ulong currentPageNo, ushort pageSize, string? sortDescription = null);

        /// <summary>
        /// 根据标识取得实体。
        /// </summary>
        /// <param name="name">标识数据的名称。</param>
        /// <param name="id">标识。</param>
        /// <returns>实体，未找到时为null。</returns>
        DataTable GetEntity(string name, object id);
    }
}
