// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/8 14:31:00 LeeZheng 新建。
//==============================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Compete.Extensions
{
    public static class StringExtensions
    {
        public static IDictionary<string, string> ToParameterDictionary(this string str)
        {
            IDictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] keyValuePair;
            foreach (var parameter in str.Split(';'))
            {
                keyValuePair = parameter.Split('=');
                if (keyValuePair.LongLength > 1L)
                    result.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
            }
            return result;
        }

        public static string[] SplitRemoveEmpty(this string str, params char[] separators)
        {
            var count = (from separator in separators
                         where separator == ' '
                         select separator).LongCount();
            if (count == 0L)
                _ = separators.Append(' ');

            return str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool BeginsWith(this string str, string value)
        {
            if (str.Length < value.Length)
                return false;

            return str[..value.Length] == value;
        }

        public static string ToCamelCase(this string str)
        {
            IList<char> upperCase = new List<char>();
            foreach (var ch in str)
                if (ch >= 'A' && ch <= 'Z')
                    upperCase.Add(ch);
                else
                    break;

            if (upperCase.Count == 0)
                return str;

            string result = string.Empty;
            if (upperCase.Count == 1)
                result = upperCase[0].ToString().ToLower();
            else
            {
                for (int index = 0; index < upperCase.Count - 1; index++)
                    result += upperCase[index].ToString().ToLower();
                result += upperCase[upperCase.Count - 1];
            }

            result += str[upperCase.Count..];

            return result;
        }

        public static T ToEnum<T>(this string enumValue) => (T)Enum.Parse(typeof(T), enumValue);

        public static bool IsUpper(this char ch) => ch >= 'A' && ch <= 'Z';

        public static bool IsLower(this char ch) => ch >= 'a' && ch <= 'z';

        //private static string Recovery(this string str) => str.Replace("\\u002B", "+").Replace("\\u0026", "&").Replace("\\u003D", "=").Replace("\\u002F", "/");
        private static string Recovery(this string str) => str.Replace("\\u002B", "+");

#if JAVA_LANGUAGE
        public static string ToJsonString(this object obj) => JsonSerializer.Serialize(obj, new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), });

        public static string ToBase64String(this byte[] inArray) => Convert.ToBase64String(inArray);
#else
        public static string ToJsonString(this object obj) => JsonSerializer.Serialize(obj, new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), }).Recovery();

        public static string ToBase64String(this byte[] inArray) => Convert.ToBase64String(inArray).Recovery();
#endif
    }
}
