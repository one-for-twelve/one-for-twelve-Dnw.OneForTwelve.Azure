using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Dnw.OneForTwelve.Azure.Api.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using NSubstitute;
using Xunit;

using AuthenticationMiddleware = Dnw.OneForTwelve.Azure.Api.Middleware.AuthenticationMiddleware;

namespace Dnw.OneForTwelve.Azure.Api.Tests.Middleware;

public class AuthenticationMiddlewareTests
{
    private readonly FunctionContext _functionContext;
    private readonly AuthenticationScheme _authScheme;
    private readonly DefaultHttpContext _defaultHttpContext;
    private readonly IJwtBearerHandlerAdapter _jwtBearerHandlerAdapter;
    private readonly FunctionExecutionDelegate _next;

    private readonly AuthenticationMiddleware _middleware;
    
    public AuthenticationMiddlewareTests()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();

        _functionContext = Substitute.For<FunctionContext>();
        _functionContext.InstanceServices = serviceProvider;
        
        _authScheme = new AuthenticationScheme("x", "y", typeof(JwtBearerHandler));
        var jwtAuthSchemeProvider = Substitute.For<IJwtAuthSchemeProvider>();
        jwtAuthSchemeProvider.GetScheme().Returns(_authScheme);
        serviceProvider.GetService(typeof(IJwtAuthSchemeProvider)).Returns(jwtAuthSchemeProvider);
        
        _defaultHttpContext = new DefaultHttpContext();
        var defaultHttpContextFactory = Substitute.For<IDefaultHttpContextFactory>();
        defaultHttpContextFactory.Create().Returns(_defaultHttpContext);
        serviceProvider.GetService(typeof(IDefaultHttpContextFactory)).Returns(defaultHttpContextFactory);

        var httpRequestFeature = new HttpRequestFeature();
        var httpRequestFeatureFactory = Substitute.For<IHttpRequestFeatureFactory>();
        httpRequestFeatureFactory.Create().Returns(httpRequestFeature);
        serviceProvider.GetService(typeof(IHttpRequestFeatureFactory)).Returns(httpRequestFeatureFactory);
        
        _jwtBearerHandlerAdapter = Substitute.For<IJwtBearerHandlerAdapter>();
        serviceProvider.GetService(typeof(IJwtBearerHandlerAdapter)).Returns(_jwtBearerHandlerAdapter);

        _next = Substitute.For<FunctionExecutionDelegate>();
        
        _middleware = new AuthenticationMiddleware(defaultHttpContextFactory, httpRequestFeatureFactory, jwtAuthSchemeProvider);
    }
    
    [Fact]
    public async Task Invoke()
    {
        // Given
        _functionContext.BindingContext.BindingData["Headers"]
            .Returns("{\"Authorization\":\"Bearer token\"}");
 
        var claimsPrincipal = new ClaimsPrincipal();
        var authResult = AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, _authScheme.Name));
        _jwtBearerHandlerAdapter.AuthenticateAsync().Returns(authResult);
        
        // When
        await _middleware.Invoke(_functionContext, _next);

        // Then
        await _jwtBearerHandlerAdapter.Received(1).InitializeAsync(_authScheme, _defaultHttpContext);
        _functionContext.Features.Received(1).Set(claimsPrincipal);
    }
    
    [Theory]
    [InlineData("{}")]
    [InlineData("{\"SomeHeader\":\"Header Value\"}")]
    [InlineData("{\"Authorization\":\"\"}")]
    public async Task Invoke_MissingToken(string headersJson)
    {
        // Given
        _functionContext.BindingContext.BindingData["Headers"]
            .Returns(headersJson);

        // When
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _middleware.Invoke(_functionContext, _next));

        // Then
        Assert.NotNull(ex);
        
        await _jwtBearerHandlerAdapter.DidNotReceive().InitializeAsync(Arg.Any<AuthenticationScheme>(), Arg.Any<HttpContext>());
        _functionContext.Features.DidNotReceive().Set(Arg.Any<ClaimsPrincipal>());
    }
    
    [Fact]
    public async Task Invoke_InvalidBase64Token()
    {
        // Given
        _functionContext.BindingContext.BindingData["Headers"]
            .Returns("{\"Authorization\":\"x.y.z\"}");

        const string failureMessage = "aMessage";
        var authResult = AuthenticateResult.Fail(failureMessage);
        _jwtBearerHandlerAdapter.AuthenticateAsync().Returns(authResult);
        
        // When
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _middleware.Invoke(_functionContext, _next));

        // Then
        Assert.NotNull(ex);
        Assert.Equal(failureMessage, ex.Message);
        
        await _jwtBearerHandlerAdapter.Received(1).InitializeAsync(_authScheme, _defaultHttpContext);
        _functionContext.Features.DidNotReceive().Set(Arg.Any<ClaimsPrincipal>());
    }
    
    [Fact]
    public async Task Invoke_AuthenticateAsyncFails()
    {
        // Given
        _functionContext.BindingContext.BindingData["Headers"]
            .Returns("{\"Authorization\":\"Bearer token\"}");

        const string failureMessage = "aMessage";
        var authResult = AuthenticateResult.Fail(failureMessage);
        _jwtBearerHandlerAdapter.AuthenticateAsync().Returns(authResult);
        
        // When
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _middleware.Invoke(_functionContext, _next));

        // Then
        Assert.NotNull(ex);
        Assert.Equal(failureMessage, ex.Message);
        
        await _jwtBearerHandlerAdapter.Received(1).InitializeAsync(_authScheme, _defaultHttpContext);
        _functionContext.Features.DidNotReceive().Set(Arg.Any<ClaimsPrincipal>());
    }
}