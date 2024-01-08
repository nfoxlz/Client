using System.Data;

namespace Compete.Mis.Models
{
    public sealed class PagingDataQueryResult
    {
        public DataSet? Data { get; set; }

        public ulong Count { get; set; }

        public ulong PageNo { get; set; }
    }
}
