using System.Collections.Generic;

namespace Compete.Mis.Frame.Services.ServiceModels
{
    public sealed class PagingQueryResult : QueryResult
    {
        //public IList<Models.SimpleData>? Data { get; set; }

        public ulong Count { get; set; }

        public ulong PageNo { get; set; }

        public Models.PagingDataQueryResult ToDataResult() => new() { Data = Data is null ? null : MemoryData.DataCreator.Create(Data!), Count = Count, PageNo = PageNo };
    }
}
