using LogisticManagementApp.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Tests.Helpers;

internal static class TestDbContextFactory
{
    public static LogisticAppDbContext Create(string? databaseName = null)
    {
        var connection = new SqliteConnection($"Data Source={(databaseName ?? Guid.NewGuid().ToString())};Mode=Memory;Cache=Shared");
        connection.Open();

        var options = new DbContextOptionsBuilder<LogisticAppDbContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;

        var dbContext = new LogisticAppDbContext(options);
        dbContext.Database.EnsureCreated();
        return dbContext;
    }
}
