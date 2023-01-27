using System.Text.Json;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Dnw.OneForTwelve.Azure.Api.Middleware
{
    public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly IDefaultHttpContextFactory _defaultHttpContextFactory;
        private readonly IHttpRequestFeatureFactory _httpRequestFeatureFactory;
        private readonly IJwtAuthSchemeProvider _jwtAuthSchemeProvider;

        public AuthenticationMiddleware(
            IDefaultHttpContextFactory defaultHttpContextFactory,
            IHttpRequestFeatureFactory httpRequestFeatureFactory, 
            IJwtAuthSchemeProvider jwtAuthSchemeProvider)
        {
            _defaultHttpContextFactory = defaultHttpContextFactory;
            _httpRequestFeatureFactory = httpRequestFeatureFactory;
            _jwtAuthSchemeProvider = jwtAuthSchemeProvider;
        }
        
        public async Task Invoke(
            FunctionContext context,
            FunctionExecutionDelegate next)
        {
            var token = TryGetTokenFromHeaders(context);
            if (string.IsNullOrWhiteSpace(token))
            {
                // Unable to get token from headers
                throw new UnauthorizedAccessException("No Bearer token in Authorization header");
            }
            
            var defaultHttpContext = _defaultHttpContextFactory.Create();
            
            var httpRequestFeature = _httpRequestFeatureFactory.Create();
            httpRequestFeature.Headers.Add(HeaderNames.Authorization, new StringValues($"Bearer {token}"));
            defaultHttpContext.Features.Set<IHttpRequestFeature>(httpRequestFeature);
            
            // We have to resolve this as a scoped service.
            var jwtBearerHandlerAdapter = context.InstanceServices.GetRequiredService<IJwtBearerHandlerAdapter>();
            await jwtBearerHandlerAdapter.InitializeAsync(_jwtAuthSchemeProvider.GetScheme(), defaultHttpContext);
            var result = await jwtBearerHandlerAdapter.AuthenticateAsync();

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException(result.Failure?.Message);
            }
            
            context.Features.Set(result.Principal);
            
            await next(context);
        }

        private static string? TryGetTokenFromHeaders(FunctionContext context)
        {
            var headersJson = context.BindingContext.BindingData["Headers"] as string;
            var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(headersJson!) ?? new Dictionary<string, string>();
            return headers.TryGetValue("Authorization", out var authHeader) ? authHeader.Split(" ").LastOrDefault() : null;
        }
    }
}
