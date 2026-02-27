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
                entity.ToTable("iconResources", "FENGSUIJB");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Content).HasColumnName("content").IsRequired();
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.Category).HasColumnName("category");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });
        }
    }
}