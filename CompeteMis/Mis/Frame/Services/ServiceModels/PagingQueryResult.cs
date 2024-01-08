using System.Collections.Generic;

namespace Compete.Mis.Frame.Services.ServiceModels
{
    public sealed class PagingQueryResult
    {
        public IList<Models.SimpleDataTable>? Data { get; set; }

        public ulong Count { get; set; }

        public ulong PageNo { get; set; }

        public Models.PagingDataQueryResult ToDataResult() => new() { Data = Data == null ? null : MemoryData.DataCreator.Create(Data!), Count = Count, PageNo = PageNo };
    }
}
