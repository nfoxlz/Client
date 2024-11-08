using System.Data;

namespace Compete.Mis.Provider
{
    internal class DataConvertHelper
    {
        public static DataSet Convert(Frame.Services.ServiceModels.QueryResult queryResult)
        {
            if (queryResult.ErrorNo == -1)
                throw new Exceptions.BusinessException(GlobalCommon.GetMessage(queryResult.Message ?? GlobalCommon.GetMessage("Message.BusinessError")));
            return MemoryData.DataCreator.Create(queryResult.Data!);
        }
    }
}
