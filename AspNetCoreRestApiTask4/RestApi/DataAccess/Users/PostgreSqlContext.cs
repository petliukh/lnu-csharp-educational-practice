namespace RestApi.DataAccess.Users;

using Microsoft.EntityFrameworkCore;
using RestApi.Models.Users;

public class UserPostgreSqlContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserPostgreSqlContext(DbContextOptions<UserPostgreSqlContext> options)
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
