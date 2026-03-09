using AutoMapper;
using HMS.Application.DTOs;
using HMS.Core.Entities;

public class HotelService : IHotelService
{
    private readonly IGenericRepository<Hotel> _hotelRepository;

    private readonly IMapper _mapper;

    public HotelService(
        IGenericRepository<Hotel> hotelRepository,
        IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HotelDto>> GetHotels(HotelFilterDto filter)
    {
        var hotels = await _hotelRepository.GetAllAsync();

        var query = hotels.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Country))
            query = query.Where(h => h.Country == filter.Country);

        if (!string.IsNullOrEmpty(filter.City))
            query = query.Where(h => h.City == filter.City);

        if (filter.Rating.HasValue)
            query = query.Where(h => h.Rating == filter.Rating);

        return _mapper.Map<IEnumerable<HotelDto>>(query.ToList());
    }

    public async Task<HotelDto> CreateHotel(CreateHotelDto dto)
    {
        var hotel = _mapper.Map<Hotel>(dto);

        hotel.Id = Guid.NewGuid();

        await _hotelRepository.AddAsync(hotel);

        await _hotelRepository.SaveAsync();

        return _mapper.Map<HotelDto>(hotel);
    }

    public async Task<HotelDto?> UpdateHotel(Guid id, UpdateHotelDto dto)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);

        if (hotel == null)
            return null;

        hotel.Name = dto.Name;
        hotel.Address = dto.Address;
        hotel.Rating = (byte)dto.Rating;

        _hotelRepository.Update(hotel);

        await _hotelRepository.SaveAsync();

        return _mapper.Map<HotelDto>(hotel);
    }

    public async Task<HotelDto?> GetHotelById(Guid id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);

        if (hotel == null)
            return null;

        return _mapper.Map<HotelDto>(hotel);
    }

    public async Task<bool> DeleteHotel(Guid id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);

        if (hotel == null)
            return false;

        if (hotel.Rooms.Any())
            throw new Exception("Hotel has rooms and cannot be deleted");

        var hasReservations = hotel.Rooms
            .SelectMany(r => r.ReservationRooms)
            .Any();

        if (hasReservations)
            throw new Exception("Hotel has active reservations");

        _hotelRepository.Delete(hotel);

        await _hotelRepository.SaveAsync();

        return true;
    }
}