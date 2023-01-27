using Microsoft.AspNetCore.Http;
using Xunit;
using DefaultHttpContextFactory = Dnw.OneForTwelve.Azure.Api.Middleware.DefaultHttpContextFactory;

namespace Dnw.OneForTwelve.Azure.Api.Tests.Middleware;

public class DefaultHttpContextFactoryTests
{
    [Fact]
    public void Create()
    {
        // Given
        var factory = new DefaultHttpContextFactory();

        // When
        var actual = factory.Create();

        // Then
        Assert.IsType<DefaultHttpContext>(actual);
    }
}