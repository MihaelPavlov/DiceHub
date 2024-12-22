using DH.Database.Connector;
using DH.Statistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DH.Statistics.Data;

public class StatisticsDbContext : TenantDbContext
{
    public StatisticsDbContext(DbContextOptions options, Assembly entityAssembly) : base(options, entityAssembly)
    {
    }

    public DbSet<Game> Games { get; set; } = default!;
}
