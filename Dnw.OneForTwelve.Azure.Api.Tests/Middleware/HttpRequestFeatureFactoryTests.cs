using Dnw.OneForTwelve.Azure.Api.Middleware;
using Microsoft.AspNetCore.Http.Features;
using Xunit;

namespace Dnw.OneForTwelve.Azure.Api.Tests.Middleware;

public class HttpRequestFeatureFactoryTests
{
    [Fact]
    public void Create()
    {
        // Given
        var factory = new HttpRequestFeatureFactory();

        // When
        var actual = factory.Create();

        // Then
        Assert.IsType<HttpRequestFeature>(actual);
    }
}