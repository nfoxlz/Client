using System.Collections.Generic;
using System.Collections.ObjectModel;
using Compete.MemoryData;

namespace Compete.Mis.Developer.Models
{
    internal sealed class DataModel
    {
        public IDictionary<string, DbTable> EntitySettings { get; set; } = new Dictionary<string, DbTable>();

        public ICollection<DataColumnSetting> ColumnSettings { get; set; } = new ObservableCollection<DataColumnSetting>();
    }
}
