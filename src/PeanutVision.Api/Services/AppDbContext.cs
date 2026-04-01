using Microsoft.EntityFrameworkCore;

namespace PeanutVision.Api.Services;

public sealed class AppDbContext : DbContext
{
    public DbSet<CapturedImage> CapturedImages => Set<CapturedImage>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CapturedImage>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.FilePath).HasMaxLength(1000).IsRequired();
            entity.Property(c => c.ThumbnailPath).HasMaxLength(1000);
            entity.Property(c => c.Format).HasMaxLength(10).IsRequired();
            entity.Property(c => c.Tags).HasDefaultValue("[]").IsRequired();
            entity.Property(c => c.Notes).HasDefaultValue(string.Empty).IsRequired();
            entity.HasIndex(c => c.CapturedAt);
        });
    }
}
