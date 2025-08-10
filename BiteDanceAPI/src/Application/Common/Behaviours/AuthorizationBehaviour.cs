using System.Reflection;
using BiteDanceAPI.Application.Common.Exceptions;
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Constants;

namespace BiteDanceAPI.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse>(
    IUserService userService,
    ICurrentUser currentUser
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var authorizeAttributes = request
            .GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();
        
        if (authorizeAttributes.Count == 0)
        {
            return await next();
        }
 
        // Must be authenticated user
        if (currentUser.Id == null)
        {
            throw new UnauthorizedAccessException();
        }

        // Role-based authorization
        var requireSupplier = authorizeAttributes.Any(a => a.RequireSupplier);
        var requireAdmin = authorizeAttributes.Any(a => a.RequireAdmin);
        var requireSuperAdmin = authorizeAttributes.Any(a => a.RequireSuperAdmin);
        var denySupplier = authorizeAttributes.Any(a => a.DenySupplier);
        if (requireSupplier)
        {
            var user = await userService.GetFromDatabaseOrCreateAsync(cancellationToken);
            if (user is null || !user.IsSupplier)
            {
                throw new ForbiddenAccessException();
            }
        }
        if (denySupplier)
        {
            var user = await userService.GetFromDatabaseOrCreateAsync(cancellationToken);
            if (user is not null && user.IsSupplier)
            {
                throw new ForbiddenAccessException(); // 👈 Không cho supplier truy cập
            }
        }

        if (requireAdmin)
        {
            var user = await userService.GetFromDatabaseOrCreateAsync(cancellationToken);
            if (user is null || !user.IsAdmin)
            {
                throw new ForbiddenAccessException();
            }
        }

        if (requireSuperAdmin)
        {
            if (
                currentUser is null
                || currentUser.Email?.ToLower() != AuthorizationConst.SuperAdminEmail
            )
            {
                throw new ForbiddenAccessException();
            }
        }

        // Policy-based authorization
        // var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
        // if (authorizeAttributesWithPolicies.Any())
        // {
        //     foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
        //     {
        //         var authorized = await identityService.AuthorizeAsync(currentUser.Id, policy);
        //
        //         if (!authorized)
        //         {
        //             throw new ForbiddenAccessException();
        //         }
        //     }
        // }

        // User is authorized / authorization not required
        return await next();
    }
}
