using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HenryCsharpTemplate.Domain.Converters;
using HenryCsharpTemplate.Domain.Models.Base;

namespace HenryCsharpTemplate.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, TimeProvider timeProvider) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // we don't want ef core SQL logs.
        optionsBuilder.UseLoggerFactory(
            LoggerFactory.Create(builder =>
                builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Error)
            )
        );

        optionsBuilder.ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        AddUtcConverterForDateTimeProps(builder);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken token = default)
    {
        var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));
        foreach (var entityEntry in entries)
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;
            ((BaseEntity)entityEntry.Entity).DateUpdated = now;

            if (entityEntry.State == EntityState.Added)
            {
                ((BaseEntity)entityEntry.Entity).DateCreated = now;
            }
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, token);
    }

    private static void AddUtcConverterForDateTimeProps(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var dateTimeProperties = entityType.GetProperties().Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

            foreach (var property in dateTimeProperties)
            {
                property.SetValueConverter(new DateTimeToUtcConverter());
            }
        }
    }
}
