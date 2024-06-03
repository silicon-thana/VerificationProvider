using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using VerificationProvider.Data.Contexts;
using VerificationProvider.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var configuration = context.Configuration;
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? Environment.GetEnvironmentVariable("DefaultConnection");

        Console.WriteLine($"Connection String: {connectionString}");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("The ConnectionString property has not been initialized.");
        }

        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IVerificationService, VerificationService>();
        services.AddScoped<IVerificationCleanerService, VerificationCleanerService>();
        services.AddScoped<IValidateVerificationCodeService, ValidateVerificationCodeService>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var migrations = context.Database.GetPendingMigrations();
        if (migrations != null && migrations.Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"ERROR : Program.cs - Migration of Database :: {ex.Message}");
    }
}

host.Run();
