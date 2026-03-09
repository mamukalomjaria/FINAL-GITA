using HMS.Core.Entities;
using HMS.Infrastructure.Data;
namespace HMS.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepository<Hotel> Hotels { get; }
        public IGenericRepository<Room> Rooms { get; }
        public IGenericRepository<Guest> Guests { get; }
        public IGenericRepository<Reservation> Reservations { get; }
        public IGenericRepository<ApplicationUser> Users { get; }


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Hotels = new GenericRepository<Hotel>(context);
            Rooms = new GenericRepository<Room>(context);
            Guests = new GenericRepository<Guest>(context);
            Reservations = new GenericRepository<Reservation>(context);
            Users = new GenericRepository<ApplicationUser>(context);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}