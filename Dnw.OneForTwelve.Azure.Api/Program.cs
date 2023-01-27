using Dnw.OneForTwelve.Azure.Api.Extensions;
using Dnw.OneForTwelve.Azure.Api.Middleware;
using Dnw.OneForTwelve.Core.Extensions;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
        builder.UseMiddleware<AuthenticationMiddleware>();
    })
    .ConfigureServices(services =>
    {
        services.AddFirebaseJwtAuth();
        
        services.ConfigureJsonSerializerOptions();
        services.AddGameServices();
    })
    .Build();

host.Run();