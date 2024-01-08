using System.Data;

namespace Compete.Mis.Plugins
{
    public sealed class SplitData
    {
        public DataTable? AddedTable { get; set; }

        public DataTable? DeletedTable { get; set; }

        public DataTable? ModifiedTable { get; set; }

        public DataTable? ModifiedOriginalTable { get; set; }
    }
}
