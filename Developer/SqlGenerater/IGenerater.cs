using System.Collections.Generic;

namespace Compete.Mis.Developer.SqlGenerater
{
    internal interface IGenerater
    {
        string GenerateCreateSql(Models.DbTable table);

        string GenerateAll(IEnumerable<Models.DbTable> tables);
    }
}
