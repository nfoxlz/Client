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

            var methodName = targetMethod.Name;
#if !JAVA_LANGUAGE
            #region Iris（Go 语言）服务器特别处理。（换为其它服务器时，应注释掉这些代码。）

            if (methodName.Length > 3 && methodName.StartsWith("Get") && methodName[3].IsUpper())
                methodName = methodName[3..];
            //else if (methodName.Length > 4 && methodName.StartsWith("Post") && methodName[4].IsUpper())
            //    methodName = methodName[4..];

            string name = string.Empty;
            foreach (var ch in methodName)
                name += ch.IsUpper() ? "/" + ch.ToString().ToLower() : ch;
            methodName = name[1..];

            #endregion Iris（Go 语言）服务器特别处理。
#endif

            Dictionary<string, object?>? parameters;
            if (args is null)
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
                if (targetMethod?.ReturnType == typeof(void) || targetMethod?.ReturnType == null)
                {
                    Global.ServiceHelper.Post($"{className}/{methodName}", parameters);
                    return null;
                }
                else
                    return Global.ServiceHelper.Post(targetMethod!.ReturnType, $"{className}/{methodName}", parameters);
                //{
                //    var result = Global.ServiceHelper.Post(targetMethod!.ReturnType, $"{className}/{methodName}", parameters);
                //    return result;
                //}
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new Exceptions.PlatformException(GlobalCommon.GetMessage("Message.ServiceError", className, methodName), exception);
            }
        }
    }
}
