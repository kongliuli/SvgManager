using Microsoft.EntityFrameworkCore;
using SvgManagerMvp.Models;

namespace SvgManagerMvp.Data
{
    public class SvgDbContext : DbContext
    {
        public DbSet<SvgData> IconResources { get; set; }

        public SvgDbContext(DbContextOptions<SvgDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SvgData>(entity =>
            {
                entity.ToTable("IconResources");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("datetime('now')");
            });
        }
    }
}