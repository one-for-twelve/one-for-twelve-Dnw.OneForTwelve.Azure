using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Dnw.OneForTwelve.Azure.Api.Middleware;

public interface IJwtAuthSchemeProvider
{
    AuthenticationScheme GetScheme();
}

public class JwtAuthSchemeProvider : IJwtAuthSchemeProvider
{
    public AuthenticationScheme GetScheme() => new AuthenticationScheme("Bearer", "Bearer", typeof(JwtBearerHandler));
}