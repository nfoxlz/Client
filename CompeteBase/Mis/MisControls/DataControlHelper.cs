// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/21 9:37:41 LeeZheng 新建。
//==============================================================
using System;
using System.Collections.Generic;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// DataControlHelper 类。
    /// </summary>
    public static class DataControlHelper
    {
        /// <summary>
        /// 将 <see cref="string"/> 格式的参数改为字<see cref="IDictionary<string, string>"/>式。
        /// </summary>
        /// <param name="parameters"><see cref="string"/> 格式的参数。</param>
        /// <returns><see cref="IDictionary<string, string>"/> 格式的参数。</returns>
        public static IDictionary<string, string>? ConvertParameters(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
                return null;

            //var result = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);    // 键不区分大小写。
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);  // 键不区分大小写。

            string[] parameter;
            //Parallel.ForEach(parameters.Split(';'), pair =>
            //{
            //    parameter = pair.Split('=');
            //    if (parameter.LongLength < 2L)
            //        return;

            //    result.TryAdd(parameter[0].Trim(), parameter[1].Trim());
            //});
            foreach (var pair in parameters.Split(';'))
            {
                parameter = pair.Split('=');
                if (parameter.LongLength < 2L)
                    continue;

                result.Add(parameter[0].Trim(), parameter[1].Trim());
            }

            return result;
        }

        public static string ConvertParameters(IDictionary<string, string> parameters)
        {
            if (parameters.Count == 0)
                return string.Empty;
            var result = string.Empty;
            foreach (var pair in parameters)
                result += $"{pair.Key}={pair.Value};";
            return result[..^1];
        }
    }
}
