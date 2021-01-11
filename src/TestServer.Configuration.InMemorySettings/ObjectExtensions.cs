using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestServer.Configuration.InMemorySettings
{
    internal static class ObjectExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> AsInMemoryCollection(this object instance)
        {
            var list = new List<KeyValuePair<string, string>>();

            foreach(var property in instance.GetType().GetRelevantProperties())
            {
                var value = property.GetValue(instance);
                if(value == null)
                    continue;

                list.AddRange(ExtractProperties(property, property.Name, value));
            }

            return list;
        }

        private static IEnumerable<KeyValuePair<string, string>> ExtractProperties(PropertyInfo propertyInfo,
                                                                                   string key,
                                                                                   object instance)
        {
            if(propertyInfo.PropertyType.IsSimpleType())
            {
                return new List<KeyValuePair<string, string>> {new(key, instance.ToString()!)};
            }

            if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
            {
                return ExtractValuesFromCollection(instance, key);
            }

            IEnumerable<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

            return propertyInfo.PropertyType
                               .GetRelevantProperties()
                               .Aggregate(list, ExtractValue);

            IEnumerable<KeyValuePair<string, string>> ExtractValue(IEnumerable<KeyValuePair<string, string>> current,
                                                                   PropertyInfo property)
            {
                var value = property.GetValue(instance);
                var keyValue = $"{key}:{property.Name}";

                if (value is ICollection collection)
                {
                    return ExtractValuesFromCollection(collection, keyValue);
                }

                return value == null
                           ? current
                           : current.Concat(ExtractProperties(property, keyValue, value));
            }

            static IEnumerable<KeyValuePair<string, string>> ExtractValuesFromCollection(object current, string key)
            {
                return (current as IEnumerable)?.Cast<object?>()
                                         .Select((item, index)
                                                     => new KeyValuePair<string, string>($"{key}:{index}", item!.ToString()!))
                       ?? new List<KeyValuePair<string, string>>();
            }
        }

        private static bool IsSimpleType(this Type type)
        {
            return type.IsValueType ||
                   type.IsPrimitive ||
                   new[]
                   {
                       typeof(string),
                       typeof(decimal),
                       typeof(DateTime),
                       typeof(DateTimeOffset),
                       typeof(TimeSpan),
                       typeof(Guid),
                       typeof(Uri)
                   }.Contains(type) ||
                   Convert.GetTypeCode(type) != TypeCode.Object;
        }

        private static IEnumerable<PropertyInfo> GetRelevantProperties(this IReflect type)
            => type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                   .Where(info => info.CanRead && info.CanWrite);
    }
}