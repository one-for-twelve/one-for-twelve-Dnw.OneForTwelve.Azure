using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Dnw.OneForTwelve.Azure.Api.Middleware;

/// <summary>
/// Todo: write tests
/// </summary>
public class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occured: {msg}", ex.Message);
            
            var httpReqData = await context.GetHttpRequestDataAsync();
            
            if (httpReqData != null)
            {
                var httpStatusCode = HttpStatusCode.InternalServerError;
                
                if (ex is UnauthorizedAccessException)
                {
                    httpStatusCode = HttpStatusCode.Unauthorized;
                }
                
                var httpResponse = httpReqData.CreateResponse(httpStatusCode);
                // You need to explicitly pass the status code in WriteAsJsonAsync method.
                // https://github.com/Azure/azure-functions-dotnet-worker/issues/776
                await httpResponse.WriteAsJsonAsync(new { ex.Message }, httpResponse.StatusCode);
                
                var invocationContext = context.GetInvocationResult();
                invocationContext.Value = httpResponse;
                
                return;
            }

            throw;
        }
    }
}