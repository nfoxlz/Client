using System.Collections.Generic;
using Compete.MemoryData;

namespace Compete.Mis.Developer.Models
{
    internal sealed class DbTable : DataObject
    {
        public bool IsView { get; set; }

        public required ICollection<DataColumnSetting> ColumnSettings { get; set; }

        public ICollection<string>? KeySettings { get; set; }

        public IDictionary<string, ICollection<string>>? IndexSettings { get; set; }

    }
}
