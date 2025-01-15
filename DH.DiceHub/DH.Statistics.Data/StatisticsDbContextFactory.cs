using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DH.Statistics.Data;

public class StatisticsDbContextFactory : IDesignTimeDbContextFactory<StatisticsDbContext>
{
    public StatisticsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StatisticsDbContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=DH.DiceHub.Statistics;User Id=postgres;Password=1qaz!QAZ;",
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(StatisticsDbContext).Assembly.FullName));

        var assembly = Assembly.GetExecutingAssembly();
        return new StatisticsDbContext(optionsBuilder.Options, assembly);
    }
}