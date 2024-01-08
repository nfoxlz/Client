using System.Reflection;

namespace Compete.Mis.Frame.Services.WebApi
{
    internal class WpfWebApiServiceProxy : WebApiServiceProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            var cursor = ViewHelper.BeginProcess();
            try
            {
                return base.Invoke(targetMethod, args);
            }
            finally
            {
                ViewHelper.EndProcess(cursor);
            }
        }
    }
}
