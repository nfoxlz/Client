// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/4/18 13:21:48 LeeZheng 新建。
//==============================================================
using System.Data;

namespace Compete.Mis.Developer.PowerDesigner
{
    /// <summary>
    /// MySQLConverter 类。
    /// </summary>
    internal sealed class MySQLConverter : IDataTypeConverter
    {
        public string DefaultCurrentDateTime => "CURRENT_TIMESTAMP(6)";

        public string DefaultCurrentDate => "CURRENT_TIMESTAMP(6)";

        public DbType DataTypeToDbType(string dataType)
        {
            string type = dataType.ToLower();
            switch (type)
            {
                case "binary(16)":
                    return DbType.Guid;
                case "bigint":
                    return DbType.Int64;
                case "int":
                    return DbType.Int32;
                case "smallint":
                    return DbType.Int16;
                case "tinyint":
                    return DbType.Byte;
                case "boolean":
                case "bool":
                    return DbType.Boolean;
                case "float":
                    return DbType.Double;
                case "real":
                    return DbType.Single;
                case "decimal(15,2)":
                    return DbType.Currency;
                case "datetime":
                case "smalldatetime":
                case "datetime(6)":
                    return DbType.DateTime;
                //return DbType.DateTime2;
                case "date":
                    return DbType.Date;
                case "time":
                    return DbType.Time;
                default:
                    if (type.Length > 4 && type[..4] == "char")
                        return DbType.StringFixedLength;

                    if (type.Length > 4 && type[..4] == "text")
                        return DbType.String;

                    if (type.Length >= 7)
                        switch (type[..7])
                        {
                            case "decimal":
                            case "numeric":
                                return DbType.Decimal;
                            case "varchar":
                                return DbType.String;
                        }

                    if (type.Length > 6 && type[..6] == "binary" || type.Length > 9 && type[..9] == "varbinary")
                        return DbType.Binary;

                    return DbType.String;
            }
        }
    }
}