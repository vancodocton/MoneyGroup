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

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(o => o.Issuer)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany<User>()
                .WithMany()
                .UsingEntity<OrderConsumer>(join =>
                {
                    join.HasKey(oc => new { oc.OrderId, oc.ConsumerId });
                    join.HasOne(oc => oc.Order)
                        .WithMany(o => o.Consumers)
                        .HasForeignKey(oc => oc.OrderId)
                        .OnDelete(DeleteBehavior.Restrict);
                    join.HasOne(oc => oc.Consumer)
                        .WithMany()
                        .HasForeignKey(oc => oc.ConsumerId)
                        .OnDelete(DeleteBehavior.Restrict);
                });
        });
    }
}