using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Domain.Constants;
using BiteDanceAPI.Infrastructure.Data;
using BiteDanceAPI.Infrastructure.Data.Interceptors;
using BiteDanceAPI.Infrastructure.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace BiteDanceAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(
            connectionString,
            message: "Connection string 'DefaultConnection' not found."
        );

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>(
            (sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                options.UseSqlServer(connectionString).EnableSensitiveDataLogging();
            }
        );

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>()
        );

        services.AddScoped<ApplicationDbContextInitialiser>();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(
                options =>
                {
                    configuration.Bind("AzureAd", options);
                    options.TokenValidationParameters.NameClaimType = "name";
                },
                options =>
                {
                    configuration.Bind("AzureAd", options);
                }
            );

        // services.AddAuthorization(config =>
        // {
        //     config.AddPolicy("AuthZPolicy", policyBuilder =>
        //         policyBuilder.Requirements.Add(new ScopeAuthorizationRequirement() { RequiredScopesConfigurationKey = $"AzureAd:Scopes" }));
        // });

        services.AddAuthorizationBuilder();

        // services
        //     .AddIdentityCore<ApplicationUser>()
        //     .AddRoles<IdentityRole>()
        //     .AddEntityFrameworkStores<ApplicationDbContext>()
        //     .AddApiEndpoints();

        services.AddSingleton(TimeProvider.System);

        services.AddTransient<IEmailService, AzureEmailService>();

        return services;
    }
}
