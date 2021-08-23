using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Configuration.Extensions.InMemorySettings
{
    internal static class ObjectExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> AsInMemoryCollection(this object instance,
                                                                                     PropertyInfoFilter? propertyInfoFilter = null,
                                                                                     Action<IDictionary<string, string>>? collectionGenerated = null)
        {
            var settings = propertyInfoFilter ?? PropertyInfoFilter.Default;

            var list = new List<KeyValuePair<string, string>>();

            foreach(var property in instance.GetType().GetRelevantProperties(settings))
            {
                var value = property.GetValue(instance);
                if(value == null)
                    continue;

                list.AddRange(ExtractProperties(property, property.Name, value, settings));
            }

            collectionGenerated?.Invoke(list.ToDictionary(pair => pair.Key,
                                                          pair => pair.Value));
            return list;
        }

        private static IEnumerable<KeyValuePair<string, string>> ExtractProperties(PropertyInfo propertyInfo,
                                                                                   string key,
                                                                                   object instance,
                                                                                   PropertyInfoFilter propertyInfoFilter)
        {
            if(propertyInfo.PropertyType.IsSimpleType())
            {
                return new List<KeyValuePair<string, string>> { new(key, instance.ToString()!) };
            }

            if(typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
            {
                return ExtractValuesFromCollection(instance, key);
            }

            IEnumerable<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

            return propertyInfo.PropertyType
                               .GetRelevantProperties(propertyInfoFilter)
                               .Aggregate(list, ExtractValue);

            IEnumerable<KeyValuePair<string, string>> ExtractValue(IEnumerable<KeyValuePair<string, string>> current,
                                                                   PropertyInfo property)
            {
                var value = property.GetValue(instance);
                var keyValue = $"{key}:{property.Name}";

                if(value is ICollection collection)
                {
                    return current.Concat(ExtractValuesFromCollection(collection, keyValue));
                }

                return value == null
                           ? current
                           : current.Concat(ExtractProperties(property, keyValue, value, propertyInfoFilter));
            }

            static IEnumerable<KeyValuePair<string, string>> ExtractValuesFromCollection(object current, string key)
                => (current as IEnumerable)?.Cast<object?>()
                                           .Select((item, index)
                                                       => new KeyValuePair<string, string>($"{key}:{index}", item!.ToString()!))
                   ?? new List<KeyValuePair<string, string>>();
        }

        private static bool IsSimpleType(this Type type)
            => type.IsValueType ||
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

        private static IEnumerable<PropertyInfo> GetRelevantProperties(this IReflect type,
                                                                       PropertyInfoFilter propertyInfoFilter)
            => type.GetProperties(propertyInfoFilter.BindingFlags)
                   .Where(propertyInfoFilter.PropertyFilter);
    }
}