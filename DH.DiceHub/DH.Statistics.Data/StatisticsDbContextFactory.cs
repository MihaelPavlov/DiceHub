using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DH.Statistics.Data;

public class StatisticsDbContextFactory : IDesignTimeDbContextFactory<StatisticsDbContext>
{
    public StatisticsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StatisticsDbContext>()
            .UseSqlServer("Server=(local);Database=DH.DiceHub.Statistics;Trusted_Connection=True;Encrypt=False",
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(StatisticsDbContext).Assembly.FullName));

        var assembly = Assembly.GetExecutingAssembly();
        return new StatisticsDbContext(optionsBuilder.Options, assembly);
    }
}