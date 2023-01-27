using Microsoft.AspNetCore.Http;

namespace Dnw.OneForTwelve.Azure.Api.Middleware;

public interface IDefaultHttpContextFactory
{
    DefaultHttpContext Create();
}

public class DefaultHttpContextFactory : IDefaultHttpContextFactory
{
    public DefaultHttpContext Create()
    {
        return new DefaultHttpContext();
    }
}