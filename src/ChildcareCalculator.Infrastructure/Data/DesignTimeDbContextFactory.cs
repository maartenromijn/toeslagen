using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ChildcareCalculator.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // For development, use SQLite
        // For production, this would be configured via dependency injection
        optionsBuilder.UseSqlite("Data Source=childcare_calculator.db");

        // Enable sensitive data logging for development
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
