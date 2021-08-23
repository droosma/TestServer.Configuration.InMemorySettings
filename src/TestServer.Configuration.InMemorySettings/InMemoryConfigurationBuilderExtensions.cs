using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

namespace Configuration.Extensions.InMemorySettings
{
    public static class InMemoryConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddSettings<T>(this IConfigurationBuilder configurationBuilder,
                                                           T instance,
                                                           PropertyInfoFilter? propertyInfoFilter = null,
                                                           Action<IDictionary<string, string>>? collectionGenerated = null)
        {
            if(instance == null)
                throw new ArgumentNullException(nameof(instance), "instance is required");

            return configurationBuilder.AddInMemoryCollection(instance.AsInMemoryCollection(propertyInfoFilter:propertyInfoFilter,
                                                                                            collectionGenerated:collectionGenerated));
        }
    }
}