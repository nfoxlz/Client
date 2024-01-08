using System.Data;

namespace Compete.Mis.Developer.PowerDesigner
{
    internal sealed class PostgreSQLConverter : IDataTypeConverter
    {
        public string DefaultCurrentDateTime => "CURRENT_TIMESTAMP";

        public string DefaultCurrentDate => "CURRENT_DATE";

        public DbType DataTypeToDbType(string dataType)
        {
            string type = dataType.ToUpper();
            switch (type)
            {
                case "BYTEA(16)":
                    return DbType.Guid;
                case "INT8":
                case "SERIAL":
                case "SERIAL8":
                    return DbType.Int64;
                case "INT4":
                    return DbType.Int32;
                case "INT2":
                    return DbType.Int16;
                //case "tinyint":
                //    return DbType.Byte;
                case "BOOL":
                    return DbType.Boolean;
                case "FLOAT8":
                    return DbType.Double;
                case "FLOAT4":
                    return DbType.Single;
                case "MONEY":
                    return DbType.Currency;
                case "TIMESTAMP":
                case "TIMESTAMP WITH TIME ZONE":
                    return DbType.DateTime;
                //return DbType.DateTime2;
                case "DATE":
                    return DbType.Date;
                case "TIME":
                case "TIME WITH TIME ZONE":
                    return DbType.Time;
                default:
                    if (type.Length > 4 && type[..4] == "CHAR")
                        return DbType.StringFixedLength;

                    if (type.Length > 4 && type[..4] == "TEXT")
                        return DbType.String;

                    if (type.Length >= 7)
                        switch (type[..7])
                        {
                            case "DECIMAL":
                            case "NUMERIC":
                                return DbType.Decimal;
                            case "VARCHAR":
                                return DbType.String;
                        }

                    if (type.Length > 5 && type[..5] == "BYTEA")
                        return DbType.Binary;

                    return DbType.String;
            }
        }
    }
}
