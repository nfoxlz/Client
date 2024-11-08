using System.Collections.Generic;

namespace Compete.Mis.Frame.Services.ServiceModels
{
    public class QueryResult : Common.Result
    {
        public IList<Models.SimpleData>? Data { get; set; }
    }
}
