using HMS.Application.DTOs;

public interface IReservationService
{
    Task<ReservationDto> CreateReservation(Guid hotelId, CreateReservationDto dto);
    Task<bool> UpdateReservation(Guid reservationId, UpdateReservationDto dto);
    Task<bool> DeleteReservation(Guid reservationId);
    Task<List<ReservationDto>> GetReservations(Guid hotelId);
}