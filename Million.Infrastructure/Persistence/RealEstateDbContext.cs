using Microsoft.EntityFrameworkCore;
using Million.Domain.Entities;

namespace Million.Infrastructure.Persistence
{
    public class RealEstateDbContext : DbContext
    {
        public RealEstateDbContext(DbContextOptions<RealEstateDbContext> options)
            : base(options)
        {
        }

        public DbSet<Owner> Owner => Set<Owner>();
        public DbSet<Property> Property => Set<Property>();
        public DbSet<PropertyImage> PropertyImage => Set<PropertyImage>();
        public DbSet<PropertyTrace> PropertyTrace => Set<PropertyTrace>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.HasKey(e => e.IdOwner);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasKey(e => e.IdProperty);  // Configuración explícita de la clave primaria
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.Price).HasMaxLength(255);
                entity.Property(e => e.CodeInternal).HasMaxLength(50);
                entity.Property(e => e.Year).HasMaxLength(4);
                // Relación con Owner
                entity.HasOne(e => e.Owner)
                      .WithMany()  // Si un Owner puede tener muchas Properties
                      .HasForeignKey(e => e.IdOwner);  // Relacionamos IdOwner como clave foránea
            });

            modelBuilder.Entity<PropertyImage>(entity =>
            {
                entity.HasKey(e => e.IdPropertyImage);
                entity.HasOne(e => e.Property)
                      .WithMany(p => p.Images)
                      .HasForeignKey(e => e.IdProperty); 
            });

            modelBuilder.Entity<PropertyTrace>(entity =>
            {
                entity.HasKey(e => e.IdPropertyTrace);
                entity.HasOne(e => e.Property)
                      .WithMany(p => p.Traces)
                      .HasForeignKey(e => e.IdProperty); 
            });
        }
    }
}
