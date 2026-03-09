using HMS.Application.DTOs;
using HMS.Core.Entities;
using HMS.Infrastructure.UnitOfWork;

namespace HMS.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unit;

        public ReservationService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<List<ReservationDto>> SearchReservations(
            Guid? hotelId,
            Guid? guestId,
            Guid? roomId,
            DateTime? date)
        {
            var reservations = await _unit.Reservations.GetAllAsync();

            var query = reservations.AsQueryable();

            if (hotelId.HasValue)
                query = query.Where(r =>
                    r.ReservationRooms.Any(rr => rr.Room.HotelId == hotelId));

            if (guestId.HasValue)
                query = query.Where(r => r.GuestId == guestId);

            if (roomId.HasValue)
                query = query.Where(r =>
                    r.ReservationRooms.Any(rr => rr.RoomId == roomId));

            if (date.HasValue)
                query = query.Where(r =>
                    r.CheckInDate <= date && r.CheckOutDate >= date);

            return query.Select(r => new ReservationDto
            {
                Id = r.Id,
                GuestId = r.GuestId,
                CheckinDate = r.CheckInDate,
                CheckoutDate = r.CheckOutDate,
                RoomIds = r.ReservationRooms.Select(rr => rr.RoomId).ToList()
            }).ToList();
        }

        public async Task<ReservationDto> CreateReservation(Guid hotelId, CreateReservationDto dto)
        {
            if (dto.CheckinDate < DateTime.Today)
                throw new Exception("Check-in cannot be in the past");

            if (dto.CheckoutDate <= dto.CheckinDate)
                throw new Exception("Checkout must be after check-in");

            var rooms = await _unit.Rooms.GetAllAsync();

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                GuestId = dto.GuestId,
                CheckInDate = dto.CheckinDate,
                CheckOutDate = dto.CheckoutDate,
                ReservationRooms = new List<ReservationRoom>()
            };

            foreach (var roomId in dto.RoomIds)
            {
                var room = rooms.FirstOrDefault(r => r.Id == roomId && r.HotelId == hotelId);

                if (room == null)
                    throw new Exception("Room does not belong to this hotel");

                var reservations = await _unit.Reservations.GetAllAsync();

                var overlapping = reservations.Any(r =>
                    r.ReservationRooms.Any(rr =>
                        rr.RoomId == roomId &&
                        dto.CheckinDate < r.CheckOutDate &&
                        dto.CheckoutDate > r.CheckInDate));

                if (overlapping)
                    throw new Exception($"Room {roomId} already reserved");

                reservation.ReservationRooms.Add(new ReservationRoom
                {
                    RoomId = roomId
                });
            }

            await _unit.Reservations.AddAsync(reservation);

            await _unit.SaveAsync();

            return new ReservationDto
            {
                Id = reservation.Id,
                GuestId = reservation.GuestId,
                CheckinDate = reservation.CheckInDate,
                CheckoutDate = reservation.CheckOutDate,
                RoomIds = reservation.ReservationRooms.Select(r => r.RoomId).ToList()
            };
        }

        public async Task<bool> UpdateReservation(Guid reservationId, UpdateReservationDto dto)
        {
            var reservations = await _unit.Reservations.GetAllAsync();

            var reservation = reservations.FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
                return false;

            if (dto.CheckinDate < DateTime.Today)
                throw new Exception("Check-in cannot be in the past");

            if (dto.CheckoutDate <= dto.CheckinDate)
                throw new Exception("Checkout must be after check-in");

            foreach (var room in reservation.ReservationRooms)
            {
                var overlapping = reservations.Any(r =>
                    r.Id != reservationId &&
                    r.ReservationRooms.Any(rr =>
                        rr.RoomId == room.RoomId &&
                        dto.CheckinDate < r.CheckOutDate &&
                        dto.CheckoutDate > r.CheckInDate));

                if (overlapping)
                    throw new Exception($"Room {room.RoomId} already reserved in that period");
            }

            reservation.CheckInDate = dto.CheckinDate;
            reservation.CheckOutDate = dto.CheckoutDate;

            _unit.Reservations.Update(reservation);

            await _unit.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteReservation(Guid reservationId)
        {
            var reservations = await _unit.Reservations.GetAllAsync();

            var reservation = reservations.FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
                return false;

            _unit.Reservations.Delete(reservation);

            await _unit.SaveAsync();

            return true;
        }

        public async Task<List<ReservationDto>> GetReservations(Guid hotelId)
        {
            var reservations = await _unit.Reservations.GetAllAsync();

            var query = reservations
                .Where(r => r.ReservationRooms.Any(rr => rr.Room.HotelId == hotelId));

            return query.Select(r => new ReservationDto
            {
                Id = r.Id,
                GuestId = r.GuestId,
                CheckinDate = r.CheckInDate,
                CheckoutDate = r.CheckOutDate,
                RoomIds = r.ReservationRooms.Select(rr => rr.RoomId).ToList()
            }).ToList();
        }
    }
}