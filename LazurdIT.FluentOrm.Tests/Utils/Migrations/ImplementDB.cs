using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace LazurdIT.FluentOrm.Tests.Utils.Migrations;

internal class ImplementDB
{
    public static void Up(string connectionString, Func<IMigrationRunnerBuilder, IMigrationRunnerBuilder> addServerFn)
    {
        using var serviceProvider = CreateServices(connectionString, addServerFn);
        using var scope = serviceProvider.CreateScope();
        // Put the database update into a scope to ensure
        // that all resources will be disposed.
        UpDatabase(scope.ServiceProvider);
    }

    public static void Down(string connectionString, Func<IMigrationRunnerBuilder, IMigrationRunnerBuilder> addServerFn)
    {
        using var serviceProvider = CreateServices(connectionString, addServerFn);
        using var scope = serviceProvider.CreateScope();
        // Put the database update into a scope to ensure
        // that all resources will be disposed.
        DownDatabase(scope.ServiceProvider);
    }

    /// <summary>
    /// Configure the dependency injection services
    /// </summary>
    private static ServiceProvider CreateServices(string connectionString, Func<IMigrationRunnerBuilder, IMigrationRunnerBuilder> addServerFn)
    {
        return new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => addServerFn(rb).WithGlobalConnectionString(connectionString).ScanIn(typeof(AddStudentTable).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }

    /// <summary>
    /// Update the database
    /// </summary>
    private static void UpDatabase(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateUp();
    }

    /// <summary>
    /// Update the database
    /// </summary>
    private static void DownDatabase(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateDown(0);
    }
}