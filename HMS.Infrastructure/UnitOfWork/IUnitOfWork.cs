using HMS.Core.Entities;

namespace HMS.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        IGenericRepository<Hotel> Hotels { get; }
        IGenericRepository<Room> Rooms { get; }
        IGenericRepository<Guest> Guests { get; }
        IGenericRepository<Reservation> Reservations { get; }
        IGenericRepository<ApplicationUser> Users { get; }

        Task SaveAsync();
    }
}