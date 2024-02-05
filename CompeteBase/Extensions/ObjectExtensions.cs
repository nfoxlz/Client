// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/8 10:21:00 LeeZheng 新建。
//==============================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Compete.Extensions
{
    /// <summary>
    /// Extension methods for all objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 判断对象是否有指定名称的属性。
        /// </summary>
        /// <param name="obj">要判断的对象。</param>
        /// <param name="name">指定的判定属性名。</param>
        /// <returns>true为有该属性；false为没有该属性。</returns>
        public static bool HasProperty(this object obj, string name) => obj != null && (from property in obj.GetType().GetProperties()
                                                                                        where property.Name == name
                                                                                        select property).Any();

        public static object? GetPropertyValue(this object obj, string name) => obj.GetType().GetProperty(name)?.GetValue(obj);

        public static object? GetPropertyValue(this object obj, int index) => obj.GetType().GetProperties()[index].GetValue(obj);

        public static object? GetPropertyValue(this object obj, long index) => obj.GetType().GetProperties()[index].GetValue(obj);

        public static object? TryGetPropertyValue(this object obj, string name) => (obj != null && obj.HasProperty(name)) ? obj.GetType().GetProperty(name)?.GetValue(obj) : null;

        public static void SetPropertyValue(this object obj, string name, object val) => obj.GetType().GetProperty(name)?.SetValue(obj, val);

        public static void SetPropertyValue(this object obj, int index, object val) => obj.GetType().GetProperties()[index].SetValue(obj, val);

        public static void SetPropertyValue(this object obj, long index, object val) => obj.GetType().GetProperties()[index].SetValue(obj, val);

        public static bool TrySetPropertyValue(this object obj, string name, object val)
        {
            if (obj.HasProperty(name))
            {
                obj.SetPropertyValue(name, val);
                return true;
            }

            return false;
        }

        public static Type? GetPropertyType(this object obj, string name)
        {
            var info = obj.GetType().GetProperty(name);
            if (info == null)
                return null;
            else
                return info.PropertyType;
        }

        public static long PropertyCount(this object obj) => obj.GetType().GetProperties().LongLength;

        public static IDictionary<string, object?> ToDictionary(this object obj)
        {
            var result = new Dictionary<string, object?>();

            if (obj != null)
                foreach (var propertyInfo in obj.GetType().GetProperties())
                    result.Add(propertyInfo.Name, propertyInfo.GetValue(obj));

            return result;
        }

        /// <summary>
        /// Converts an object to another using AutoMapper library. Creates a new object of <see cref="TDestination"/>.
        /// There must be a mapping between objects before calling this method.
        /// </summary>
        /// <typeparam name="TDestination">Type of the destination object</typeparam>
        /// <param name="source">Source object</param>
        public static TDestination? DynamicMapTo<TDestination>(this object source)
        {
            TDestination? destination = (TDestination?)Activator.CreateInstance(typeof(TDestination));
            if (destination != null)
                source.DynamicMapTo(destination);
            return destination;
        }

        public static void DynamicMapTo(this object source, object destination)
        {
            var sourceProperties = from property in source.GetType().GetProperties()
                                   where property.CanRead
                                   select property;

            var destinationProperties = from property in destination.GetType().GetProperties()
                                        where property.CanWrite
                                        select property;
            object? sourceValue;
            Guid? idValue;
            foreach (var sourceProperty in sourceProperties)
            {
                sourceValue = sourceProperty.GetValue(source);
                if (sourceValue == null)
                    continue;

                var destinationProperty = (from property in destinationProperties
                                           where property.Name == sourceProperty.Name
                                           select property).FirstOrDefault();
                if (destinationProperty == null)// || nullCopy && destinationProperty.CanRead && destinationProperty.GetValue(destination) != null
                    continue;

                if (destinationProperty.PropertyType.ToString() == sourceProperty.PropertyType.ToString() || destinationProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    destinationProperty.SetValue(destination, sourceValue);
                else if (sourceProperty.PropertyType == typeof(Guid) && destinationProperty == typeof(byte[]))
                    destinationProperty.SetValue(destination, ((Guid)sourceValue).ToByteArray());
                else if (sourceProperty.PropertyType == typeof(byte[]) && destinationProperty == typeof(Guid))
                    destinationProperty.SetValue(destination, new Guid((byte[])sourceValue));
                else if (sourceProperty.PropertyType == typeof(Guid?) && destinationProperty == typeof(byte[]))
                {
                    idValue = sourceValue as Guid?;
                    if (idValue.HasValue)
                        destinationProperty.SetValue(destination, idValue.Value.ToByteArray());
                }
                else if (sourceProperty.PropertyType == typeof(byte[]) && destinationProperty == typeof(Guid?))
                    destinationProperty.SetValue(destination, new Guid?(new Guid((byte[])sourceValue)));
            }
        }
    }
}
