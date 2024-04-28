using System.Collections.Generic;
using System.Collections.ObjectModel;
using Compete.MemoryData;

namespace Compete.Mis.Developer.Models
{
    internal sealed class ProjectSetting
    {
        public string OutputPath { get; set; } = "output";

        public DataModel Model { get; set; } = new DataModel();

        //public IDictionary<string, DbTable> EntitySettings { get; private set; } = new Dictionary<string, DbTable>();

        //public ICollection<Data.DataColumnSetting> ColumnSettings { get; private set; } = new ObservableCollection<Data.DataColumnSetting>();

        public ICollection<DataColumnSetting> CustomColumnSettings { get; set; } = new ObservableCollection<DataColumnSetting>();

        public DatabaseConnectionSetting? ConnectionSetting { get; set; } = new DatabaseConnectionSetting();
    }
}
