using HMS.Application.DTOs;

public interface IHotelService
{
    Task<IEnumerable<HotelDto>> GetHotels(HotelFilterDto filter);

    Task<HotelDto?> GetHotelById(Guid id);

    Task<HotelDto> CreateHotel(CreateHotelDto dto);

    Task<HotelDto?> UpdateHotel(Guid id, UpdateHotelDto dto);

    Task<bool> DeleteHotel(Guid id);
}