using HMS.Application.DTOs;

namespace HMS.Application.Services
{
    public interface IReservationService
    {
        Task<ReservationDto> CreateReservation(Guid hotelId, CreateReservationDto dto);

        Task<bool> UpdateReservation(Guid reservationId, UpdateReservationDto dto);

        Task<bool> DeleteReservation(Guid reservationId);

        Task<List<ReservationDto>> GetReservations(Guid hotelId);

        Task<List<ReservationDto>> SearchReservations(
            Guid? hotelId,
            Guid? guestId,
            Guid? roomId,
            DateTime? date);
    }
}