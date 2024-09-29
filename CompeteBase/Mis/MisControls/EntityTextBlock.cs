// ======================================================
// Compete Management Information System
// ======================================================
// 版权所有 © Compete software studio 2019 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间               作者     说明
// ------------------------------------------------------
// 1.0.0.0 2019/1/26 周六 12:20:12 LeeZheng 新建。
// ======================================================
using System.Data;

namespace Compete.Mis.MisControls
{
    public class EntityTextBlock : AbstractEntityTextBlock
    {
        protected override string? GetDisplay(DataTable entities)
            => string.IsNullOrWhiteSpace(Format) || formatMethod is null
                ? entities.Rows[0][DisplayPath].ToString()
                : formatMethod.Invoke(null, [entities.Rows[0]])?.ToString();
    }
}
