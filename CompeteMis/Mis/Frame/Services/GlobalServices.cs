using System.Reflection;

namespace Compete.Mis.Frame.Services
{
    public static class GlobalServices
    {
        //public static IAccountService AccountService { get; } = DispatchProxy.Create<IAccountService, WebApi.WebApiServiceProxy>();

        public static IFrameService FrameService { get; } = DispatchProxy.Create<IFrameService, WebApi.WpfWebApiServiceProxy>();

        //public static IDataService DataService { get; } = DispatchProxy.Create<IDataService, WebApi.WebApiServiceProxy>();

        public static IUpdateService UpdateService { get; } = new Sftp.UpdateService();
    }
}
