using Dnw.OneForTwelve.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Dnw.OneForTwelve.Azure.Api.Middleware;

public static class AuthExtensions
{
    public static void AddFirebaseJwtAuth(this IServiceCollection services)
    {
        services.AddFirebaseAuth();
        
        services.AddSingleton<IDefaultHttpContextFactory, DefaultHttpContextFactory>();
        services.AddSingleton<IHttpRequestFeatureFactory, HttpRequestFeatureFactory>();
        services.AddSingleton<IJwtAuthSchemeProvider, JwtAuthSchemeProvider>();
        services.AddScoped<IJwtBearerHandlerAdapter, JwtBearerHandlerAdapter>();
    }
}