using System.IO;
using System.Linq;
using Azure.Core.Serialization;
using Dnw.OneForTwelve.Azure.Api.Extensions;
using Dnw.OneForTwelve.Core.Extensions;
using Dnw.OneForTwelve.Core.Models;
using Dnw.OneForTwelve.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Dnw.OneForTwelve.Azure.Api.IntegrationTests;

public class IntegrationTests
{
    private readonly HttpRequestData _request;
    private readonly GameApi _apiFunction;
    
    public IntegrationTests()
    {
        var context = Substitute.For<FunctionContext>();
        _request = Substitute.For<HttpRequestData>(context);
        var response = Substitute.For<HttpResponseData>(context);

        var services = new ServiceCollection();
        services.AddSingleton(Options.Create(new WorkerOptions{Serializer = new JsonObjectSerializer()}));
        services.ConfigureJsonSerializerOptions();
        services.AddGameServices();
        
        var serviceProvider = services.BuildServiceProvider();
        context.InstanceServices.ReturnsForAnyArgs(serviceProvider);

        _request.Headers.ReturnsForAnyArgs(new HttpHeadersCollection());
        response.Headers.ReturnsForAnyArgs(new HttpHeadersCollection());
        response.Body.ReturnsForAnyArgs(new MemoryStream());
        _request.CreateResponse().ReturnsForAnyArgs(response);
        
        var logger = Substitute.For<ILogger<GameApi>>();
        var gameService = serviceProvider.GetRequiredService<IGameService>();

        _apiFunction = new GameApi(logger, gameService);
    }
    
    [Fact]
    public void GetGame()
    {
        // Given
        var language = Languages.Dutch.ToString();
        var strategy = QuestionSelectionStrategies.Random.ToString();

        // When
        var actual = _apiFunction.StartGame(_request, language, strategy);

        // Then
        Assert.NotNull(actual);
        Assert.Equal(12, actual.Questions.Count());
    }
}