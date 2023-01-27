using Microsoft.AspNetCore.Http.Features;

namespace Dnw.OneForTwelve.Azure.Api.Middleware;

public interface IHttpRequestFeatureFactory
{
    HttpRequestFeature Create();
}

public class HttpRequestFeatureFactory : IHttpRequestFeatureFactory
{
    public HttpRequestFeature Create()
    {
        return new HttpRequestFeature();
    }
}