using System.Diagnostics.CodeAnalysis;

namespace BiteDanceAPI.Web.Infrastructure;

public class CustomErrorResponse
{
    public int Status { get; set; }
    public string Title { get; set; } = default!;
    public string Detail { get; set; } = default!;
    public IDictionary<string, string[]> Errors { get; set; } = default!;
}

public static class EndpointExtensions
{
    public static IEndpointConventionBuilder WithDefaultResponseTypes(
        this RouteHandlerBuilder builder
    )
    {
        return builder
            .Produces(StatusCodes.Status400BadRequest, typeof(CustomErrorResponse))
            .Produces(StatusCodes.Status500InternalServerError, typeof(CustomErrorResponse));
    }
}

public static class IEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapGet(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern = ""
    )
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapGet(pattern, handler).WithName(handler.Method.Name).WithDefaultResponseTypes();

        return builder;
    }

    public static IEndpointRouteBuilder MapPost(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern = ""
    )
    {
        Guard.Against.AnonymousMethod(handler);

        builder
            .MapPost(pattern, handler)
            .WithName(handler.Method.Name)
            .WithDefaultResponseTypes()
            .DisableAntiforgery(); // disabled for file uploading

        return builder;
    }

    public static IEndpointRouteBuilder MapPut(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern
    )
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapPut(pattern, handler).WithName(handler.Method.Name).WithDefaultResponseTypes();

        return builder;
    }

    public static IEndpointRouteBuilder MapDelete(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern
    )
    {
        Guard.Against.AnonymousMethod(handler);

        builder
            .MapDelete(pattern, handler)
            .WithName(handler.Method.Name)
            .WithDefaultResponseTypes();

        return builder;
    }
}
