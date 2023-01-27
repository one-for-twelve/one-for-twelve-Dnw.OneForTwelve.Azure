using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Dnw.OneForTwelve.Azure.Api.Extensions;

public static class JsonSerializerExtensions
{
    [UsedImplicitly]
    public static void ConfigureJsonSerializerOptions(this IServiceCollection services)
    {
        services.Configure<JsonSerializerOptions>(options => 
        {
            options.ConfigureDefaults();
        });
    }

    [UsedImplicitly]
    public static void ConfigureDefaults(this JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.Converters.Add(new JsonStringEnumConverter());
    }
}