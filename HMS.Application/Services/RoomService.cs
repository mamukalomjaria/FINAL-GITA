using HMS.Application.DTOs;
using HMS.Core.Entities;
using HMS.Infrastructure.UnitOfWork;

namespace HMS.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unit;

        public RoomService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<List<RoomDto>> GetRooms(Guid hotelId, double? minPrice, double? maxPrice)
        {
            var rooms = await _unit.Rooms.GetAllAsync();

            var query = rooms.Where(r => r.HotelId == hotelId);

            if (minPrice.HasValue)
                query = query.Where(r => r.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(r => r.Price <= maxPrice.Value);

            return query.Select(r => new RoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Price = r.Price,
                HotelId = r.HotelId
            }).ToList();
        }

        public async Task<List<RoomDto>> SearchAvailableRooms(
            Guid hotelId,
            double? minPrice,
            double? maxPrice,
            DateTime? date)
        {
            var rooms = await _unit.Rooms.GetAllAsync();

            var query = rooms.Where(r => r.HotelId == hotelId);

            if (minPrice.HasValue)
                query = query.Where(r => r.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(r => r.Price <= maxPrice.Value);

            if (date.HasValue)
            {
                query = query.Where(room =>
                    !room.ReservationRooms.Any(rr =>
                        rr.Reservation.CheckInDate <= date.Value &&
                        rr.Reservation.CheckOutDate >= date.Value));
            }

            return query.Select(r => new RoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Price = r.Price,
                HotelId = r.HotelId
            }).ToList();
        }

        public async Task<RoomDto?> GetRoom(Guid hotelId, Guid roomId)
        {
            var rooms = await _unit.Rooms.GetAllAsync();

            var room = rooms.FirstOrDefault(r => r.Id == roomId && r.HotelId == hotelId);

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
            var hotels = await _unit.Hotels.GetAllAsync();

            if (!hotels.Any(h => h.Id == hotelId))
                throw new Exception("Hotel not found");

            if (dto.Price <= 0)
                throw new Exception("Room price must be greater than 0");

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Price = dto.Price,
                HotelId = hotelId
            };

            await _unit.Rooms.AddAsync(room);

            await _unit.SaveAsync();

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
            var rooms = await _unit.Rooms.GetAllAsync();

            var room = rooms.FirstOrDefault(r => r.Id == roomId && r.HotelId == hotelId);

            if (room == null)
                return false;

            if (dto.Price <= 0)
                throw new Exception("Room price must be greater than 0");

            room.Name = dto.Name;
            room.Price = dto.Price;

            _unit.Rooms.Update(room);

            await _unit.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteRoom(Guid hotelId, Guid roomId)
        {
            var rooms = await _unit.Rooms.GetAllAsync();

            var room = rooms.FirstOrDefault(r => r.Id == roomId && r.HotelId == hotelId);

            if (room == null)
                return false;

            var hasActiveReservation = room.ReservationRooms
                .Any(rr => rr.Reservation.CheckOutDate >= DateTime.Today);

            if (hasActiveReservation)
                throw new Exception("Room has active or future reservations");

            _unit.Rooms.Delete(room);

            await _unit.SaveAsync();

            return true;
        }
    }
}