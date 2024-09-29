using System;
using System.Collections.Generic;

namespace Compete.Mis.Runtime.Security
{
    internal sealed class SignHelper(string password)
    {
        private readonly string password = password;

        private void AddAdditionalParameters(List<string> parameterList)
        {
            parameterList.Add(string.Format("timestamp={0}", DateTime.Now.ToString("yyyyMMdd")));
            parameterList.Add(string.Format("password={0}", password));
        }

        public IDictionary<string, object?>? GenerateSignParameter(IDictionary<string, object?>? parameters)
        {
            if (parameters is null)
                return null;

            var result = new Dictionary<string, object?>(parameters);
            var parameterList = new List<string>();
            foreach (var parameter in parameters)
                parameterList.Add(string.Format("{0}={1}", parameter.Key, parameter.Value));
            AddAdditionalParameters(parameterList);
            result.Add(Constants.SignParameterName, PasswordHelper.Encrypt(string.Join("&", parameterList)));

            return result;
        }

        //public IDictionary<string, object>? GenerateSignParameter(object? parameters)
        //{
        //    if (parameters is null)
        //        return null;

        //    var parameterList = new List<string>();
        //    var result = new Dictionary<string, object>();
        //    var changedpParameters = new Dictionary<string, object>();
        //    object parameterValue;
        //    foreach (var property in parameters.GetType().GetProperties())
        //    {
        //        parameterValue = property.GetValue(parameters)!;
        //        if (parameterValue is not null && (parameterValue is IDictionary || parameterValue is IEnumerable<Models.Entity>) || parameterValue is IEnumerable<long>)
        //        {
        //            changedpParameters.Add(property.Name, parameterValue);
        //            continue;
        //        }

        //        result.Add(property.Name, parameterValue ?? "");
        //        parameterList.Add(string.Format("{0}={1}", property.Name, parameterValue));
        //    }

        //    AddAdditionalParameters(parameterList);

        //    result.Add(Constants.SignParameterName, PasswordHelper.Encrypt(string.Join("&", parameterList)));

        //    foreach (var parameter in changedpParameters)
        //        result.Add(parameter.Key, parameter.Value);

        //    return result;
        //}

        public bool Verify(IDictionary<string, object> parameters)
        {
            if (parameters is null)
                return true;

            var parameterList = new List<string>();
            //var properties = parameters.GetType().GetProperties();
            string? sign = null;
            foreach (var parameter in parameters)
            {
                if (parameter.Value is not null)
                    continue;

                if (parameter.Key == Constants.SignParameterName)
                    sign = parameter.Value?.ToString();
                else
                    parameterList.Add(string.Format("{0}={1}", parameter.Key, parameter.Value));
            }

            AddAdditionalParameters(parameterList);

            return sign is null || PasswordHelper.Verify(string.Join("&", parameterList), sign);
        }

        public bool Verify(string data, string sign) =>
            PasswordHelper.Verify(data + string.Format("&timestamp={0}&password={1}", DateTime.Now.ToString("yyyyMMdd"), password), sign);
    }
}
