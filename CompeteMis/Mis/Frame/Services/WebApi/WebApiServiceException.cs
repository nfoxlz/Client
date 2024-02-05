using System.Net.Http;

namespace Compete.Mis.Frame.Services.WebApi
{
    internal class WebApiServiceException(HttpResponseMessage responseMessage) : Exceptions.PlatformException()
    {
        public HttpResponseMessage ResponseMessage { get; init; } = responseMessage;
    }
}
