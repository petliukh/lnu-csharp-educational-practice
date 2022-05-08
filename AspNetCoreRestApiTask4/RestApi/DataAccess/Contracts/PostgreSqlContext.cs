namespace RestApi.DataAccess.Contracts;

using Microsoft.EntityFrameworkCore;

using RestApi.Models.Contracts;

public class ContractPostgreSqlContext : DbContext
{
    public ContractPostgreSqlContext(DbContextOptions<ContractPostgreSqlContext> options)
        : base(options)
    {
    }

    public DbSet<Contract> Contracts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    public override int SaveChanges()
    {
        ChangeTracker.DetectChanges();
        return base.SaveChanges();
    }
}