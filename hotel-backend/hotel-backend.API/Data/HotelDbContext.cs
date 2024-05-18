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
                .HasMany(c => c.Rooms)
                .WithOne(r => r.Customer)
                .HasForeignKey(r=>r.CustomerId);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Rooms);
        }
    }
}
