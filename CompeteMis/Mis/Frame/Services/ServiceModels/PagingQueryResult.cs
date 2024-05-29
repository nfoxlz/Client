using System.Collections.Generic;

namespace Compete.Mis.Frame.Services.ServiceModels
{
    public sealed class PagingQueryResult
    {
        public IDictionary<string, Models.SimpleData>? Data { get; set; }

        public ulong Count { get; set; }

        public ulong PageNo { get; set; }

        public Models.PagingDataQueryResult ToDataResult() => new() { Data = null == Data ? null : MemoryData.DataCreator.Create(Data!), Count = Count, PageNo = PageNo };
    }
}
