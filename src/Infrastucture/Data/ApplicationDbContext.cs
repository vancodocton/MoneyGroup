using Microsoft.EntityFrameworkCore;

using MoneyGroup.Core.Entities;

namespace MoneyGroup.Infrastucture.Data;
public class ApplicationDbContext
    : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderConsumer> OrderConsumers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrderConsumer>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ConsumerId });
        });
    }
}