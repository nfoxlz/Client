using System;
using System.Collections.Generic;
using System.Data;

namespace Compete.Mis.Plugins
{
    public interface IDataProvider
    {
        DataSet Query(string path, string name, IDictionary<string, object>? parameters = null);

        Models.PagingDataQueryResult PagingQuery(string path, string name, IDictionary<string, object>? parameters = default, ulong currentPageNo = 1UL, ushort pageSize = 30, string? sortDescription = null);

        Common.Result Save(string path, string name, DataSet data, Guid actionId);

        Common.Result Save(string path, string name, DataSet data) => Save(path, name, data, Guid.NewGuid());

        Common.Result Save(string path, string name, IDictionary<string, SplitData> data, Guid actionId);

        Common.Result Save(string path, string name, IDictionary<string, SplitData> data) => Save(path, name, data, Guid.NewGuid());
    }
}
