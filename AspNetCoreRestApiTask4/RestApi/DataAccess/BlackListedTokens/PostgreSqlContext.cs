namespace RestApi.DataAccess.BlackListedTokens;

using Microsoft.EntityFrameworkCore;
using RestApi.Models.BlackListedTokens;

public class BlackListedTokenPostgreSqlContext : DbContext
{
    public DbSet<BlackListedToken> BlackListedTokens { get; set; }

    public BlackListedTokenPostgreSqlContext(DbContextOptions<BlackListedTokenPostgreSqlContext> options)
        : base(options)
    {
    }

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