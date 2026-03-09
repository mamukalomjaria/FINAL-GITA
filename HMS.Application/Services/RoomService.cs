using HMS.Application.DTOs;
using HMS.Core.Entities;
using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;

        public RoomService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoomDto>> GetRooms(Guid hotelId, double? minPrice, double? maxPrice)
        {
            var query = _context.Rooms
                .Where(r => r.HotelId == hotelId)
                .AsQueryable();

            if (minPrice.HasValue)
                query = query.Where(r => r.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(r => r.Price <= maxPrice.Value);

            var rooms = await query.ToListAsync();

            return rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Price = r.Price,
                HotelId = r.HotelId
            }).ToList();
        }

        public async Task<RoomDto?> GetRoom(Guid hotelId, Guid roomId)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == roomId && r.HotelId == hotelId);

            if (room == null)
                return null;

            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Price = room.Price,
                HotelId = room.HotelId
            };
        }

        public async Task<RoomDto> CreateRoom(Guid hotelId, CreateRoomDto dto)
        {
            if (dto.Price <= 0)
                throw new Exception("Room price must be greater than 0");

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Price = dto.Price,
                HotelId = hotelId
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Price = room.Price,
                HotelId = room.HotelId
            };
        }

        public async Task<bool> UpdateRoom(Guid hotelId, Guid roomId, UpdateRoomDto dto)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == roomId && r.HotelId == hotelId);

            if (room == null)
                return false;

            if (dto.Price <= 0)
                throw new Exception("Room price must be greater than 0");

            room.Name = dto.Name;
            room.Price = dto.Price;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteRoom(Guid hotelId, Guid roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Reservation)
                .FirstOrDefaultAsync(r => r.Id == roomId && r.HotelId == hotelId);

            if (room == null)
                return false;

            var hasActiveReservation = room.ReservationRooms
                .Any(rr => rr.Reservation.CheckoutDate >= DateTime.Today);

            if (hasActiveReservation)
                throw new Exception("Room has active or future reservations");

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}