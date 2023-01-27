using System.Security.Claims;
using Dnw.OneForTwelve.Core.Models;
using Dnw.OneForTwelve.Core.Services;
using JetBrains.Annotations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Dnw.OneForTwelve.Azure.Api;

public record GameApi(ILogger<GameApi> Logger, IGameService GameService)
{
    [UsedImplicitly]
    [Function(nameof(StartGame))]
    public Game? StartGame(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "games/{language:alpha}/{strategy:alpha}")] HttpRequestData req,
        string language, 
        string strategy)
    {
        var claimsPrincipal = req.FunctionContext.Features.Get<ClaimsPrincipal?>(); 
        var userName = claimsPrincipal?.Identity?.Name ?? "anonymous";
        Logger.LogInformation("UserName: {userName}, Language: {language}, Strategy: {strategy}", userName, language, strategy);
        var game = GameService.Start(Enum.Parse<Languages>(language), Enum.Parse<QuestionSelectionStrategies>(strategy));

        return game;
    }
}