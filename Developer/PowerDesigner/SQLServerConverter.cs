// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/4/18 13:22:48 LeeZheng 新建。
//==============================================================
using System.Data;

namespace Compete.Mis.Developer.PowerDesigner
{
    /// <summary>
    /// SQLServerConverter 类。
    /// </summary>
    internal sealed class SQLServerConverter : IDataTypeConverter
    {
        public string DefaultCurrentDateTime => "getdate()";

        public string DefaultCurrentDate => "getdate()";

        public DbType DataTypeToDbType(string dataType)
        {
            string type = dataType.ToLower();
            switch (type)
            {
                case "uniqueidentifier":
                    return DbType.Guid;
                case "bigint":
                    return DbType.Int64;
                case "int":
                    return DbType.Int32;
                case "smallint":
                    return DbType.Int16;
                case "tinyint":
                    return DbType.Byte;
                case "bit":
                    return DbType.Boolean;
                case "float":
                    return DbType.Double;
                case "real":
                    return DbType.Single;
                case "money":
                case "smallmoney":
                    return DbType.Currency;
                case "datetime":
                case "smalldatetime":
                    return DbType.DateTime;
                case "datetime2":
                    return DbType.DateTime2;
                case "date":
                    return DbType.Date;
                case "time":
                    return DbType.Time;
                case "xml":
                    return DbType.Xml;
                case "timestamp":
                    return DbType.Binary;
                default:
                    if (type.Length > 5 && type[..5] == "nchar")
                        return DbType.StringFixedLength;

                    if (type.Length > 4 && type[..4] == "char")
                        return DbType.AnsiStringFixedLength;

                    if (type.Length > 4 && type[..4] == "text")
                        return DbType.AnsiString;

                    if (type.Length >= 7)
                        switch (type[..7])
                        {
                            case "decimal":
                            case "numeric":
                                return DbType.Decimal;
                            case "varchar":
                                return DbType.AnsiString;
                        }

                    if (type == "image" || type.Length > 6 && type[..6] == "binary" || type.Length > 9 && type[..9] == "varbinary")
                        return DbType.Binary;

                    return DbType.String;
            }
        }
    }
}
