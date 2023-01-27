using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Dnw.OneForTwelve.Azure.Api.Middleware;

public interface IJwtBearerHandlerAdapter
{
    Task InitializeAsync(AuthenticationScheme scheme, HttpContext context);
    Task<AuthenticateResult> AuthenticateAsync();
}

public class JwtBearerHandlerAdapter : IJwtBearerHandlerAdapter
{
    private readonly JwtBearerHandler _jwtBearerHandler;

    public JwtBearerHandlerAdapter(JwtBearerHandler jwtBearerHandler)
    {
        _jwtBearerHandler = jwtBearerHandler;
    }

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        return _jwtBearerHandler.InitializeAsync(scheme, context);
    }

    public Task<AuthenticateResult> AuthenticateAsync()
    {
        return _jwtBearerHandler.AuthenticateAsync();
    }
}