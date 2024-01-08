using Microsoft.EntityFrameworkCore;
using Geolocation.Api.Data.Entities;
using Geolocation.API.Data.Entities;

namespace Geolocation.Api.Data.Context
{
    public class GeolocationDbContext : DbContext
    {
        public DbSet<Location> Geolocations { get; set; }

        public GeolocationDbContext(DbContextOptions<GeolocationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Latitude).IsRequired();
                entity.Property(e => e.Longitude).IsRequired();
            });
        }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                ((BaseEntity)entry.Entity).UpdatedDate = DateTime.Now;
                if(entry.State == EntityState.Added)
                    ((BaseEntity)entry.Entity).CreatedDate = DateTime.Now;
            }
            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
