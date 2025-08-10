using System.Text.Json.Serialization;
using Azure.Identity;
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Infrastructure.Data;
using BiteDanceAPI.Web.Services;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;

namespace BiteDanceAPI.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSerilog();

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IUserService, UserService>();

        services.AddHttpContextAccessor();

        services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddRazorPages();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true
        );

        services
            .AddEndpointsApiExplorer()
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        services.AddOpenApiDocument(
            (configure, sp) =>
            {
                configure.Title = "BiteDanceAPI";

                // Add JWT
                configure.AddSecurity(
                    "JWT",
                    [],
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows()
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new string(
                                    $"{configuration["AzureAd:Instance"]}{configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize"
                                ),
                                TokenUrl = new string(
                                    $"{configuration["AzureAd:Instance"]}{configuration["AzureAd:TenantId"]}/oauth2/v2.0/token"
                                ),
                                Scopes = new Dictionary<string, string>
                                {
                                    { configuration["AzureAd:Scopes"]!, "Access API" }
                                }
                            }
                        },
                    }
                );

                configure.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT")
                );

                configure.SchemaSettings.SchemaProcessors.Add(
                    new MarkAsRequiredIfNonNullableSchemaProcessor()
                );
            }
        );

        return services;
    }

    public static IServiceCollection AddKeyVaultIfConfigured(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        var keyVaultUri = configuration["KeyVaultUri"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
        }

        return services;
    }
}
