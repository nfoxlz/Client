﻿using System.Collections.Generic;

namespace Compete.Mis.Frame.Services
{
    internal interface IDataService
    {
        IDictionary<string, Models.SimpleData> Query(string path, string name, IDictionary<string, object>? parameters = default);

        //Models.SimpleDataTable QueryTable(string path, string name, IDictionary<string, object?>? parameters = default);

        ServiceModels.PagingQueryResult PagingQuery(string path, string name, IDictionary<string, object>? parameters = default, ulong currentPageNo = 1UL, ushort pageSize = 30);

        //Common.Result Save(string path, string name, IDictionary<string, Models.SimpleData> data, byte[] actionId);
        Common.Result Save(string path, string name, IDictionary<string, Models.SimpleData> data, byte[] actionId);

        Common.Result DifferentiatedSave(string path, string name, IDictionary<string, ServiceModels.SaveData> data, byte[] actionId);
    }
}
