using Compete.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Xml.Linq;

namespace Compete.Mis.Runtime.Security
{
    internal sealed class SignHelper(string password)
    {
        private readonly string password = password;

        //private void AddAdditionalParameters(IDictionary<string, object?> parameters)
        //{
        //    parameters.Add("timestamp", DateTime.Now.ToString("yyyyMMdd"));
        //    parameters.Add("Sign - password", password);
        //}


        public string GenerateSignParameter(IDictionary<string, object?> parameters)
        {
            var result = new Dictionary<string, object?>
            {
                { "timestamp", DateTime.Now.ToString("yyyyMMdd") },
                { "Sign - password", password }
            };
            //AddAdditionalParameters(result);

            //if (parameters is not null)
#if JAVA_LANGUAGE
            foreach (var pair in parameters)
                result.Add(pair.Key, pair.Value);

            var s = result.Values.ToJsonString();

            return PasswordHelper.Encrypt(result.Values.ToJsonString());
#else
            foreach ( var pair in parameters)
                    if (pair.Value is IDictionary<string, object?>)
                        result.Add(pair.Key, new SortedDictionary<string, object?>((IDictionary<string, object?>)pair.Value, StringComparer.Ordinal));
                    else if (pair.Value is null)
                        result.Add(pair.Key, string.Empty);
                    else
                        result.Add(pair.Key, pair.Value);

            //var s = result.ToJsonString();

            //return PasswordHelper.Encrypt(result.ToJsonString().Replace("\\u002B", "+"));
            return PasswordHelper.Encrypt(result.ToJsonString());
#endif
            //foreach ( var pair in parameters)
            //        if (pair.Value is IDictionary<string, object?>)
            //            result.Add(pair.Key, new SortedDictionary<string, object?>((IDictionary<string, object?>)pair.Value));
            //        else if (pair.Value is null)
            //            result.Add(pair.Key, string.Empty);
            //        else
            //            result.Add(pair.Key, pair.Value);

            ////var s = result.ToJsonString();
        }

        //public bool Verify(IDictionary<string, object> parameters)
        //{
        //    if (parameters is null)
        //        return true;

        //    string? sign = null;

        //    if (parameters.TryGetValue(Constants.SignParameterName, out var signValue))
        //    {
        //        sign = signValue?.ToString();
        //        parameters.Remove(Constants.SignParameterName);
        //    }

        //    return sign is null || PasswordHelper.Verify(parameters.ToJsonString(), sign);
        //}

        //public bool Verify(string data, string sign) =>
        //    PasswordHelper.Verify(data + string.Format("&timestamp={0}&password={1}", DateTime.Now.ToString("yyyyMMdd"), password), sign);
    }
}
