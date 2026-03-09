using HMS.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<Guest> Guests { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<ReservationRoom> ReservationRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ReservationRoom>()
                .HasKey(rr => new { rr.ReservationId, rr.RoomId });

            builder.Entity<ReservationRoom>()
                .HasOne(rr => rr.Reservation)
                .WithMany(r => r.ReservationRooms)
                .HasForeignKey(rr => rr.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ReservationRoom>()
                .HasOne(rr => rr.Room)
                .WithMany(r => r.ReservationRooms)
                .HasForeignKey(rr => rr.RoomId)
                .OnDelete(DeleteBehavior.Restrict);  
        }

    }
}