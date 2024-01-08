namespace Compete.Mis.Developer.SqlGenerater
{
    internal sealed class PostgreSQLGenerater : GeneraterBase
    {
        protected override string AutoIncrementType => "SERIAL8";
    }
}
