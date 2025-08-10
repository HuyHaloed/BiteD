using Microsoft.Extensions.Logging;
using ValidationException = BiteDanceAPI.Application.Common.Exceptions.ValidationException;

namespace BiteDanceAPI.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await next();
        }
        catch (ValidationException ex)
        {
            var requestName = typeof(TRequest).Name;

            logger.LogError(
                ex,
                "BiteDanceAPI Request: Unhandled Exception for Request {Name}\n {@Request}\n {@exception}",
                requestName,
                request,
                ex.Errors
            );

            throw;
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            logger.LogError(
                ex,
                "BiteDanceAPI Request: Unhandled Exception for Request {Name}\n {@Request}\n {@exception}",
                requestName,
                request,
                ex
            );

            throw;
        }
    }
}
