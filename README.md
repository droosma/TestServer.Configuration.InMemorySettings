# TestServer.Configuration.InMemorySettings

## Background

To streamline configuration in my `Microsoft.NET.Sdk.Web` projects I usually create POCO like objects like this:

``` CSharp
public class Settings 
{
    public string ApplicationName { get; set; }
    public SwaggerSettings SwaggerSettings { get; set; } = new();
    
    //.. snip for brevity ..
    
    public static Settings From(IConfiguration configuration)
    {
        var settings = new Settings();
        configuration.Bind(settings);

        return settings;
    }
}

public class Startup
{
    private readonly Settings _settings;
    public Startup(IConfiguration configuration)
    {
        _settings = Settings.From(configuration);
        //.. snip for brevity ..
    }
}
```

I also really love [Microsoft.AspNetCore.TestHost](https://www.nuget.org/packages/Microsoft.AspNetCore.TestHost/).
While using this tool I have run into the whole configuration of settings yack shaving a couple of times.
To address this problem Microsoft have created `MemoryConfigurationBuilderExtensions` in [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/) which adds the following extension method:

``` CSharp
public static IConfigurationBuilder AddInMemoryCollection(this IConfigurationBuilder configurationBuilder, IEnumerable<KeyValuePair<string, string>> initialData);
```

This is a pretty clean solution, would it not be that I now have to create a `IEnumerable<KeyValuePair<string, string>>` and keep that in sink.

This is my solution to that problem. As I already have a `Settings` object I just want to convert this to the required `IEnumerable<KeyValuePair<string, string>>`

## Usage

``` CSharp
var settings = new Settings 
                   {
                        ApplicationName = "ApplicationName"
                        //.. snip for brevity ..
                   };
TestServer testServer = new WebHostBuilder().UseEnvironment("Test")
                                            .UseStartup<Startup>()
                                            .ConfigureAppConfiguration(builder => 
                                            {
                                                builder.AddSettings(settings)
                                            });
```

## Disclamer

The reflection I'm using to accomplish this conversion is quite naive and past experience has tought me that there are bugs in this code.
If you find a scenario that doesn't work for you please submit a issue.
