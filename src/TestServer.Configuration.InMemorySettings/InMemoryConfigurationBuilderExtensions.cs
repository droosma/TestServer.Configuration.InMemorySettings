using System;

using Microsoft.Extensions.Configuration;

namespace TestServer.Configuration.InMemorySettings
{
    public static class InMemoryConfigurationBuilderExtensions 
    {
        public static IConfigurationBuilder AddSettings<T>(this IConfigurationBuilder configurationBuilder, T instance)
        {
            if(instance == null)
                throw new ArgumentNullException(nameof(instance), "instance is required");

            return configurationBuilder.AddInMemoryCollection(instance.AsInMemoryCollection());
        }
    }
}
