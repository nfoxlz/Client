using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Compete.Mis.Provider
{
    internal sealed class EntityDataProvider : MisControls.IEntityDataProvider
    {
        private static readonly Frame.Services.IDataService service = DispatchProxy.Create<Frame.Services.IDataService, Frame.Services.WebApi.WpfWebApiServiceProxy>();

        private const string pluginPath = "entitySelector";

        public DataTable GetEntity(string name, object id) => DataConvertHelper.Convert(service.Query(pluginPath, $"get{name}", new Dictionary<string, object> { { "id", id } })).Tables[0];

        public Models.PagingDataQueryResult Query(string name, IDictionary<string, object>? conditions, ulong currentPageNo, ushort pageSize, string? sortDescription = null)
#if JAVA_LANGUAGE
            => service.PagingQuery(pluginPath, $"query{name}", Utils.JavaHelper.Convert(conditions), currentPageNo, pageSize, sortDescription).ToDataResult();   // Java
#else
            => service.PagingQuery(pluginPath, $"query{name}", conditions, currentPageNo, pageSize, sortDescription).ToDataResult();
#endif
    }
}
