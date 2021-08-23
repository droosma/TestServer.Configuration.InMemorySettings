using System;
using System.Reflection;

namespace Configuration.Extensions.InMemorySettings
{
    public class PropertyInfoFilter
    {
        private PropertyInfoFilter(BindingFlags bindingFlags,
                                   Func<PropertyInfo, bool> propertyFilter)
        {
            BindingFlags = bindingFlags;
            PropertyFilter = propertyFilter;
        }

        public BindingFlags BindingFlags { get; }
        public Func<PropertyInfo, bool> PropertyFilter { get; }

        public static PropertyInfoFilter Default
            => new(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public,
                   info => info.CanRead && info.CanWrite);

        public static PropertyInfoFilter Create(BindingFlags bindingFlags,
                                                Func<PropertyInfo, bool> propertyFilter)
            => new(bindingFlags, propertyFilter);
    }
}