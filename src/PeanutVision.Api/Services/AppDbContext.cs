using Microsoft.EntityFrameworkCore;

namespace PeanutVision.Api.Services;

public sealed class AppDbContext : DbContext
{
    public DbSet<Session> Sessions => Set<Session>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).HasMaxLength(200).IsRequired();
            entity.Property(s => s.Notes).HasMaxLength(2000);
            entity.HasIndex(s => s.CreatedAt);
        });
    }
}
