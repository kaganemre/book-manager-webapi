using BookManager.Application.Common.Services;
using BookManager.Application.Interfaces;
using BookManager.Infrastructure.Context;
using BookManager.Infrastructure.Data;
using BookManager.Infrastructure.Identity;
using BookManager.Infrastructure.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static async Task UseInfrastructureAsync(this IApplicationBuilder app)
    {
        app.ApplicationServices.ApplyMigrations();
        await app.ApplicationServices.SeedBooksAsync();
        await app.ApplicationServices.SeedIdentityAsync();
    }

    private static void ApplyMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var pendingMigrations = dbContext.Database.GetPendingMigrations();

        if (pendingMigrations.Any())
        {
            Console.WriteLine($"Applying {pendingMigrations.Count()} pending migrations...");
            dbContext.Database.Migrate();
        }
    }
}