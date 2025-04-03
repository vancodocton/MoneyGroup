using Microsoft.EntityFrameworkCore;

using MoneyGroup.Core.Entities;

namespace MoneyGroup.Infrastructure.Data;
public class ApplicationDbContext
    : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
#pragma warning disable IL2026,IL3050
        : base(options)
#pragma warning restore IL2026,IL3050 // RequiresDynamicCodeAttribute + RequiresUnreferencedCodeAttribute
    {
    }

    protected ApplicationDbContext(DbContextOptions options)
#pragma warning disable IL2026,IL3050
        : base(options)
#pragma warning restore IL2026,IL3050 // RequiresDynamicCodeAttribute + RequiresUnreferencedCodeAttribute    
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderParticipant> OrderParticipants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(o => o.Buyer)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany<User>()
                .WithMany()
                .UsingEntity<OrderParticipant>(join =>
                {
                    join.HasKey(oc => new { oc.OrderId, oc.ParticipantId });
                    join.HasOne(oc => oc.Order)
                        .WithMany(o => o.Participants)
                        .HasForeignKey(oc => oc.OrderId)
                        .OnDelete(DeleteBehavior.Cascade);
                    join.HasOne(oc => oc.Participant)
                        .WithMany()
                        .HasForeignKey(oc => oc.ParticipantId)
                        .OnDelete(DeleteBehavior.Restrict);
                });
        });
    }
}
