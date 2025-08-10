using System.Diagnostics;
using BiteDanceAPI.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace BiteDanceAPI.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger,
    ICurrentUser currentUser) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;


            logger.LogWarning("BiteDanceAPI Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@currentUser} {@Request}",
                requestName, elapsedMilliseconds, currentUser, request);
        }

        return response;
    }
}
