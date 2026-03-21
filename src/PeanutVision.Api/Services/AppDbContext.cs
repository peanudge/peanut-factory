using Microsoft.EntityFrameworkCore;

namespace PeanutVision.Api.Services;

public sealed class AppDbContext : DbContext
{
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<CapturedImage> CapturedImages => Set<CapturedImage>();

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

        modelBuilder.Entity<CapturedImage>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.FilePath).HasMaxLength(1000).IsRequired();
            entity.Property(c => c.ThumbnailPath).HasMaxLength(1000);
            entity.Property(c => c.Format).HasMaxLength(10).IsRequired();
            entity.HasIndex(c => c.CapturedAt);
            entity.HasIndex(c => c.SessionId);
            entity.HasOne(c => c.Session)
                  .WithMany(s => s.CapturedImages)
                  .HasForeignKey(c => c.SessionId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
