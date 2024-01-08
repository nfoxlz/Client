using System.Reflection;

namespace Compete.Mis.Frame.Services
{
    internal static class GlobalServices
    {
        //public static IAccountService AccountService { get; } = DispatchProxy.Create<IAccountService, WebApi.WebApiServiceProxy>();

        public static IFrameService FrameService { get; } = DispatchProxy.Create<IFrameService, WebApi.WpfWebApiServiceProxy>();

        //public static IDataService DataService { get; } = DispatchProxy.Create<IDataService, WebApi.WebApiServiceProxy>();
    }
}
