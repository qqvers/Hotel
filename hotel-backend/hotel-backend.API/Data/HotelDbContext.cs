using hotel_backend.API.Models;
using Microsoft.EntityFrameworkCore;

namespace hotel_backend.API.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
            
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Rooms)
                .WithOne(r => r.Customer)
                .HasForeignKey(r=>r.CustomerId);

            modelBuilder.Entity<Owner>()
                .HasMany(o => o.Customers);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Rooms);
        }
    }
}
