using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Compete.Extensions
{
    internal static class CollectionExtensions
    {
        /// <summary>
        /// 合并字典
        /// </summary>
        /// <param name="first">第一个字典</param>
        /// <param name="second">第二个字典</param>
        /// <returns>合并后的数组(第一个字典+第二个字典，长度为两个字典的长度)</returns>
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second, bool isCovered = false)
        {
            //KeyValuePair<TKey, TValue>[] data = new KeyValuePair<TKey, TValue>[first.Count + second.Count];
            //first.CopyTo(data, 0);
            //second.CopyTo(data, first.Count);
            //IDictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            //foreach (var item in data)
            //    if (!result.ContainsKey(item.Key))
            //        result.Add(item);
            //return result;
            foreach (var pair in second)
                if (first.ContainsKey(pair.Key))
                {
                    if (isCovered)
                        first[pair.Key] = pair.Value;
                }
                else
                    first.Add(pair);

            return first;
        }

        public static IDictionary<TKey, TValue> ClearNull<TKey, TValue>(this IDictionary<TKey, TValue?> dic) where TKey : notnull
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var pair in dic)
                if (pair.Value is not null)
                    result.Add(pair.Key, pair.Value);
            return result;
        }

        public static IDictionary<TKey, TValue?> ToNullDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dic) where TKey : notnull
        {
            var result = new Dictionary<TKey, TValue?>();
            foreach (var pair in dic)
                result.Add(pair.Key, pair.Value);
            return result;
        }

        /// <summary>
        /// 合并数组
        /// </summary>
        /// <param name="first">第一个数组</param>
        /// <param name="second">第二个数组</param>
        /// <returns>合并后的数组(第一个数组+第二个数组，长度为两个数组的长度)</returns>
        //public static T[] Merge<T>(this T[] first, T[] second)
        //{
        //    T[] result = new T[first.LongLength + second.LongLength];
        //    first.CopyTo(result, 0L);
        //    second.CopyTo(result, first.LongLength);
        //    return result;
        //}

        /// <summary>
        /// 合并列表
        /// </summary>
        /// <param name="first">第一个列表</param>
        /// <param name="second">第二个列表</param>
        /// <returns>合并后的列表(第一个列表+第二个列表，长度为两个列表的长度)</returns>
        public static IEnumerable<T> Merge<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            var result = new List<T>(first);
            foreach (var item in second)
                result.Add(item);
            return result;
        }

        /// <summary>
        /// 插入数组
        /// </summary>
        /// <param name="first">第一个数组</param>
        /// <param name="second">第二个数组</param>
        /// <returns>合并后的数组(第一个数组+第二个数组，长度为两个数组的长度)</returns>
        public static T[] Insert<T>(this T[] first, T[] second) => second.Merge(first);

        /// <summary>
        /// 插入数组
        /// </summary>
        /// <param name="first">第一个数组</param>
        /// <param name="second">第二个数组</param>
        /// <returns>合并后的数组(第一个数组+第二个数组，长度为两个数组的长度)</returns>
        public static T[] Insert<T>(this T[] first, T item)
        {
            T[] result = new T[first.LongLength + 1L];
            result[0] = item;
            first.CopyTo(result, 1L);
            return result;
        }

        /// <summary>
        /// 插入数组
        /// </summary>
        /// <param name="first">第一个数组</param>
        /// <param name="second">第二个数组</param>
        /// <returns>合并后的数组(第一个数组+第二个数组，长度为两个数组的长度)</returns>
        public static T[] Append<T>(this T[] first, T item)
        {
            T[] result = new T[first.LongLength + 1L];
            first.CopyTo(result, 0L);
            result[first.LongLength] = item;
            return result;
        }

        ///// <summary>
        ///// 从数组中截取一部分成新的数组
        ///// </summary>
        ///// <param name="source">原数组</param>
        ///// <param name="startIndex">原数组的起始位置</param>
        ///// <returns></returns>
        //public static T[] Split<T>(this T[] source, long startIndex) => source.Split(startIndex, source.LongLength - startIndex);

        ///// <summary>
        ///// 从数组中截取一部分成新的数组
        ///// </summary>
        ///// <param name="source">原数组</param>
        ///// <param name="startIndex">原数组的起始位置</param>
        ///// <param name="endIndex">原数组的截止位置</param>
        ///// <returns></returns>
        //public static T[] Split<T>(this T[] source, long startIndex, long length)
        //{
        //    T[] result = new T[length];
        //    for (long i = 0; i < length; i++)
        //        result[i] = source[i + startIndex];
        //    return result;
        //}

        ///// <summary>
        ///// 从数组中截取一部分成新的数组
        ///// </summary>
        ///// <param name="source">原数组</param>
        ///// <param name="startIndex">原数组的起始位置</param>
        ///// <returns></returns>
        //public static T[] Split<T>(this T[] source, int startIndex) => source.Split(startIndex, source.Length - startIndex);

        ///// <summary>
        ///// 从数组中截取一部分成新的数组
        ///// </summary>
        ///// <param name="source">原数组</param>
        ///// <param name="startIndex">原数组的起始位置</param>
        ///// <param name="endIndex">原数组的截止位置</param>
        ///// <returns></returns>
        //public static T[] Split<T>(this T[] source, int startIndex, int length)
        //{
        //    T[] result = new T[length];
        //    for (int i = 0; i < length; i++)
        //        result[i] = source[i + startIndex];
        //    return result;
        //}

        public static List<TDestination?> ToDynamicMapList<TDestination>(this IEnumerable<object> list) => list.Select(source => source.DynamicMapTo<TDestination>()).ToList();

        /// <summary>
        /// 取得当前元素。
        /// </summary>
        /// <param name="enumerable">枚举器。</param>
        /// <returns>当前元素。</returns>
        public static object? Current(this IEnumerable enumerable)
        {
            if (enumerable is string)
                return enumerable;

            try
            {
                return (enumerable as IEnumerable).GetEnumerator().Current;
            }
            catch (InvalidOperationException)
            {
                var enumerator = (enumerable as IEnumerable).GetEnumerator();
                if (enumerator.MoveNext())
                    return enumerator.Current;
                else
                    return (enumerable as IEnumerable<object>)?.FirstOrDefault();
            }
        }
    }
}
