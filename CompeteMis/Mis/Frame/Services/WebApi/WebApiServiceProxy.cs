using Compete.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compete.Mis.Frame.Services.WebApi
{
    internal class WebApiServiceProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            var className = targetMethod!.DeclaringType!.Name;
            if (className.EndsWith("Service"))
                className = className[..^7];
            
            if (className.Length > 1 && className.StartsWith('I') && className[1].IsUpper())
                className = className[1..];

            Dictionary<string, object?>? parameters;
            if (args == null)
                parameters = null;
            else
            {
                parameters = [];
                var informations = targetMethod.GetParameters();
                var len = args!.LongLength;
                for (var i = 0L; i < len; i++)
                    parameters.Add(informations[i].Name!, args[i]);
            }

            try
            {
                if (targetMethod?.ReturnType == typeof(void))
                {
                    Global.ServiceHelper.Post($"{className}/{targetMethod?.Name}", parameters);
                    return null;
                }
                else
                    return Global.ServiceHelper.Post(targetMethod!.ReturnType, $"{className}/{targetMethod?.Name}", parameters);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new Exceptions.PlatformException(GlobalCommon.GetMessage("Message.ServiceError", className, targetMethod.Name), exception);
            }
        }
    }
}
