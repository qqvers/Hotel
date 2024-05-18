using hotel_backend.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace hotel_backend.API.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Owner> Owners { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .Property<DateTime>("CreatedDate");

            modelBuilder.Entity<Customer>()
                .Property<DateTime>("UpdatedDate");

            modelBuilder.Entity<Owner>()
                .Property<DateTime>("CreatedDate");

            modelBuilder.Entity<Owner>()
                .Property<DateTime>("UpdatedDate");

            modelBuilder.Entity<Room>()
                .Property<DateTime>("CreatedDate");

            modelBuilder.Entity<Room>()
                .Property<DateTime>("UpdatedDate");

        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedDate").CurrentValue = DateTime.UtcNow;
                }
                entry.Property("UpdatedDate").CurrentValue = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedDate").CurrentValue = DateTime.UtcNow;
                }
                entry.Property("UpdatedDate").CurrentValue = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
