using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dnw.OneFßorTwelve.Azure.Api.IntegrationTests.Models;

[UsedImplicitly]
public record Game(string Word, List<GameQuestion> Questions);

[UsedImplicitly]
public record GameQuestion(int Number);