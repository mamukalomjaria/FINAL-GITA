using HMS.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Guest>()
                .HasIndex(x => x.PersonalNumber)
                .IsUnique();

            modelBuilder.Entity<Guest>()
                .HasIndex(x => x.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(x => x.PersonalNumber)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(x => x.Email)
                .IsUnique();
            modelBuilder.Entity<Reservation>()
               .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.GuestId);
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Hotel)
                .WithMany(h => h.Managers)
                .HasForeignKey(u => u.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservationRoom>()
                .HasKey(rr => new { rr.ReservationId, rr.RoomId });
        }

    }
}