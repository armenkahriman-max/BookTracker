using BookTracker.Api.Application;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Wiring;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddBookTracker(this WebApplicationBuilder builder)
    {
        RegisterStorage(builder);
        RegisterHandlers(builder.Services);

        return builder;
    }

    private static void RegisterStorage(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("BookTracker")));

        builder.Services.AddScoped<IBookRepository, EfBookRepository>();
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

    private static readonly Type HandlerMarker = typeof(IHandler);
}