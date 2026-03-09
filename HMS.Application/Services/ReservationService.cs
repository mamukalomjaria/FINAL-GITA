using HMS.Application.DTOs;
using HMS.Core.Entities;
using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReservationDto> CreateReservation(Guid hotelId, CreateReservationDto dto)
        {
            if (dto.CheckinDate < DateTime.Today)
                throw new Exception("Check-in cannot be in the past");

            if (dto.CheckoutDate <= dto.CheckinDate)
                throw new Exception("Checkout must be after check-in");

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                CheckinDate = dto.CheckinDate,
                CheckoutDate = dto.CheckoutDate,
                GuestId = dto.GuestId,
                ReservationRooms = new List<ReservationRoom>()
            };

            foreach (var roomId in dto.RoomIds)
            {
                var overlapping = await _context.ReservationRooms
                    .Include(x => x.Reservation)
                    .AnyAsync(x =>
                        x.RoomId == roomId &&
                        dto.CheckinDate < x.Reservation.CheckoutDate &&
                        dto.CheckoutDate > x.Reservation.CheckinDate);

                if (overlapping)
                    throw new Exception($"Room {roomId} already reserved");

                reservation.ReservationRooms.Add(new ReservationRoom
                {
                    RoomId = roomId
                });
            }

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return new ReservationDto
            {
                Id = reservation.Id,
                GuestId = reservation.GuestId,
                CheckinDate = reservation.CheckinDate,
                CheckoutDate = reservation.CheckoutDate,
                RoomIds = reservation.ReservationRooms.Select(r => r.RoomId).ToList()
            };
        }

        public async Task<bool> UpdateReservation(Guid reservationId, UpdateReservationDto dto)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);

            if (reservation == null)
                return false;

            if (dto.CheckinDate < DateTime.Today)
                throw new Exception("Check-in cannot be in the past");

            if (dto.CheckoutDate <= dto.CheckinDate)
                throw new Exception("Checkout must be after check-in");

            reservation.CheckinDate = dto.CheckinDate;
            reservation.CheckoutDate = dto.CheckoutDate;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteReservation(Guid reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);

            if (reservation == null)
                return false;

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<ReservationDto>> GetReservations(Guid hotelId)
        {
            var reservations = await _context.Reservations
                .Include(r => r.ReservationRooms)
                .Where(r => r.ReservationRooms.Any(rr => rr.Room.HotelId == hotelId))
                .ToListAsync();

            return reservations.Select(r => new ReservationDto
            {
                Id = r.Id,
                GuestId = r.GuestId,
                CheckinDate = r.CheckinDate,
                CheckoutDate = r.CheckoutDate,
                RoomIds = r.ReservationRooms.Select(rr => rr.RoomId).ToList()
            }).ToList();
        }
    }
}