using Dnw.OneForTwelve.Azure.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Xunit;

namespace Dnw.OneForTwelve.Azure.Api.Tests.Middleware;

public class JwtAuthSchemeProviderTests
{
    [Fact]
    public void GetScheme()
    {
        // Given
        var provider = new JwtAuthSchemeProvider();

        // When
        var actual = provider.GetScheme();

        // Then
        Assert.Equal("Bearer", actual.Name);
        Assert.Equal("Bearer", actual.DisplayName);
        Assert.Equal(typeof(JwtBearerHandler), actual.HandlerType);
    }
}