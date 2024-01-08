using Compete.Extensions;
using System.Collections.Generic;
using System.Text;

namespace Compete.Mis.Developer.SqlGenerater
{
    internal abstract class GeneraterBase : IGenerater
    {
        public string GenerateCreateSql(Models.DbTable table)
        {
            var sql = new StringBuilder();
            sql.AppendLine($"-- Table: {table.Code}");
            sql.AppendLine($"CREATE TABLE {table.Code} (");
            foreach (var setting in table.ColumnSettings)
            {
                var clauses = new List<string> { setting.ColumnName };
                if (setting.IsAutoIncrement)
                    clauses.Add(AutoIncrementType);
                else
                    clauses.Add(setting.DbPhysicalType!);
                if (setting.IsRequired || !setting.AllowDBNull)
                    clauses.Add("NOT NULL");
                if (setting.DefaultValue != null)
                    if (setting.DbDataType.IsString())
                        clauses.Add($"DEFAULT \"{setting.DefaultValue}\"");
                    else
                        clauses.Add($"DEFAULT {setting.DefaultValue}");
                sql.AppendLine($"\t{string.Join(" ", clauses)},");
            }
            if (table.KeySettings != null)
                sql.AppendLine($"\tCONSTRAINT PK_{table.Code.ToUpper()} PRIMARY KEY ({string.Join(", ", table.KeySettings)})");
            sql.AppendLine(");");
            sql.AppendLine();

            sql.AppendLine($"COMMENT ON TABLE {table.Code} IS '{table.Name}';");

            foreach (var setting in table.ColumnSettings)
                sql.AppendLine($"COMMENT ON COLUMN {table.Code}.{setting.ColumnName} IS '{setting.Caption}';");

            sql.AppendLine();

            if (table.IndexSettings != null)
            {
                foreach (var setting in table.IndexSettings)
                    sql.AppendLine($"CREATE INDEX {setting.Key} ON {table.Code} ({string.Join(", ", setting.Value)});");
                sql.AppendLine();
            }

            return sql.ToString();
        }

        public string GenerateAll(IEnumerable<Models.DbTable> tables)
        {
            var sql = new StringBuilder();
            foreach (var table in tables)
                if (!table.IsView)
                    sql.Append(GenerateCreateSql(table));
            return sql.ToString();
        }

        protected abstract string AutoIncrementType { get; }
    }
}
