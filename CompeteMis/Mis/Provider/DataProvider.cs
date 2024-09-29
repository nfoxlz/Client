using Compete.Common;
using Compete.Extensions;
using Compete.Mis.Plugins;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Compete.Mis.Provider
{
    internal sealed class DataProvider : IDataProvider
    {
        private static readonly Frame.Services.IDataService service = DispatchProxy.Create<Frame.Services.IDataService, Frame.Services.WebApi.WebApiServiceProxy>();

        public Models.PagingDataQueryResult PagingQuery(string path, string name, IDictionary<string, object>? parameters = default, ulong currentPageNo = 1UL, ushort pageSize = 30)
#if JAVA_LANGUAGE
            => service.PagingQuery(path, name, Utils.JavaHelper.Convert(parameters), currentPageNo, pageSize).ToDataResult();    // Java
#else
            => service.PagingQuery(path, name, parameters, currentPageNo, pageSize).ToDataResult();
#endif

#if JAVA_LANGUAGE
        public DataSet Query(string path, string name, IDictionary<string, object>? parameters) => MemoryData.DataCreator.Create(service.Query(path, name, Utils.JavaHelper.Convert(parameters)));  // Java

        //public Result Save(string path, string name, DataSet data, Guid actionId) => service.Save(path, name, MemoryData.DataCreator.ConvertSimpleDataSet(data), actionId.ToByteArray());
#else
        public DataSet Query(string path, string name, IDictionary<string, object>? parameters) => MemoryData.DataCreator.Create(service.Query(path, name, parameters));
        //public Result Save(string path, string name, DataSet data, Guid actionId) => service.Save(path, name, MemoryData.DataCreator.ConvertSimpleDataSet(data), data.GetTableNames(), actionId.ToByteArray());
#endif
        public Result Save(string path, string name, DataSet data, Guid actionId) => service.Save(path, name, MemoryData.DataCreator.ConvertSimpleDataSet(data), actionId.ToByteArray());

        public Result Save(string path, string name, IDictionary<string, SplitData> data, Guid actionId)
        {
            var saveData = new Dictionary<string, Frame.Services.ServiceModels.SaveData>();
            Frame.Services.ServiceModels.SaveData tableSaveData;
            bool hasData;
            foreach (var tableData in data)
            {
                tableSaveData = new Frame.Services.ServiceModels.SaveData();
                hasData = false;
                if (tableData.Value.AddedTable is not null)
                {
                    tableSaveData.AddedTable = MemoryData.DataCreator.ConvertSimpleDataTable(tableData.Value.AddedTable);
                    hasData = true;
                }
                if (tableData.Value.DeletedTable is not null)
                {
                    tableSaveData.DeletedTable = MemoryData.DataCreator.ConvertSimpleDataTable(tableData.Value.DeletedTable);
                    hasData = true;
                }
                if (tableData.Value.ModifiedTable is not null)
                {
                    tableSaveData.ModifiedTable = MemoryData.DataCreator.ConvertSimpleDataTable(tableData.Value.ModifiedTable);
                    hasData = true;
                }
                if (tableData.Value.ModifiedOriginalTable is not null)
                {
                    tableSaveData.ModifiedOriginalTable = MemoryData.DataCreator.ConvertSimpleDataTable(tableData.Value.ModifiedOriginalTable);
                    hasData = true;
                }
                if (hasData)
                    saveData.Add(tableData.Key, tableSaveData);
            }

            return service.DifferentiatedSave(path, name, saveData, actionId.ToByteArray());
        }
    }
}
