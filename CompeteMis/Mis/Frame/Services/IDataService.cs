using System.Collections.Generic;

namespace Compete.Mis.Frame.Services
{
    internal interface IDataService
    {
        //IList<Models.SimpleData> Query(string path, string name, IDictionary<string, object>? parameters = default);
        ServiceModels.QueryResult Query(string path, string name, IDictionary<string, object>? parameters = default);

        //Models.SimpleDataTable QueryTable(string path, string name, IDictionary<string, object?>? parameters = default);

        ServiceModels.PagingQueryResult PagingQuery(string path, string name, IDictionary<string, object>? parameters = default, ulong currentPageNo = 1UL, ushort pageSize = 30, string? sortDescription = null);

        //Common.Result Save(string path, string name, IDictionary<string, Models.SimpleData> data, byte[] actionId);
        Common.Result Save(string path, string name, IList<Models.SimpleData> data, byte[] actionId);
//#if JAVA_LANGUAGE
//        Common.Result Save(string path, string name, IList<Models.SimpleData> data, byte[] actionId);
//#else
//        Common.Result Save(string path, string name, IList<Models.SimpleData> data, IList<string> tableNames, byte[] actionId);
//#endif

        Common.Result DifferentiatedSave(string path, string name, IDictionary<string, ServiceModels.SaveData> data, byte[] actionId);
    }
}
