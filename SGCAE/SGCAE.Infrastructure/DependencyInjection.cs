using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGCAE.Application.Abstractions;
using SGCAE.Infrastructure.Persistence;
using SGCAE.Infrastructure.Queries;
using SGCAE.Infrastructure.Repositories;

namespace SGCAE.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // Lado escritura
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ICredentialRepository, CredentialRepository>();
        services.AddScoped<IAccessPointRepository, AccessPointRepository>();
        services.AddScoped<IAccessLogRepository, AccessLogRepository>();

        // Lado lectura
        services.AddScoped<IEmployeeQueries, EmployeeQueries>();
        services.AddScoped<ICredentialQueries, CredentialQueries>();
        services.AddScoped<IAccessPointQueries, AccessPointQueries>();
        services.AddScoped<IAccessLogQueries, AccessLogQueries>();

        return services;
    }
}
