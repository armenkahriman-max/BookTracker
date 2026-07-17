using BookTracker.Api.Application;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;
using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;
using BookTracker.Api.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Security.Claims;

namespace BookTracker.Api.Wiring;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddBookTracker(
    this WebApplicationBuilder builder)
    {
        RegisterStorage(builder);
        RegisterHandlers(builder.Services);
        RegisterAuthentication(builder);

        return builder;
    }

    private static void RegisterStorage(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("BookTracker")));

        builder.Services.AddScoped<IBookRepository, EfBookRepository>();
        builder.Services.AddScoped<IMemberRepository, EfMemberRepository>();

        builder.Services.AddScoped<IPasswordHasher<Member>, PasswordHasher<Member>>();
    }

    private static void RegisterHandlers(IServiceCollection services)
    {
        var handlerTypes = HandlerMarker.Assembly
            .GetTypes()
            .Where(IsHandler);

        foreach (var type in handlerTypes)
        {
            services.AddScoped(type);
        }
    }

    private static bool IsHandler(Type type)
    {
        return type is { IsClass: true, IsAbstract: false }
            && type.IsAssignableTo(HandlerMarker);
    }

    private static void RegisterAuthentication(WebApplicationBuilder builder)
    {
        var settings = builder.Configuration
            .GetRequiredSection(JwtSettings.SectionName)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are missing.");

        if (string.IsNullOrWhiteSpace(settings.SigningKey))
        {
            throw new InvalidOperationException("JWT signing key is missing.");
        }

        builder.Services.AddSingleton(settings);
        builder.Services.AddScoped<JwtTokenGenerator>();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = settings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = settings.Audience,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(settings.SigningKey)),

                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role,

                        ClockSkew = TimeSpan.Zero
                    };
            });

        builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AuthorizationPolicies.ManageBooks,
        policy =>
            policy.RequireRole(
                nameof(MemberRole.Administrator)));

    options.AddPolicy(
        AuthorizationPolicies.ManageMembers,
        policy =>
            policy.RequireRole(
                nameof(MemberRole.Administrator)));
});
    }
    private static readonly Type HandlerMarker = typeof(IHandler);


}