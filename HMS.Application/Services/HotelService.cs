using AutoMapper;
using HMS.Application.DTOs;
using HMS.Core.Entities;
using HMS.Infrastructure.UnitOfWork;

public class HotelService : IHotelService
{
    private readonly IUnitOfWork _unit;
    private readonly IMapper _mapper;

    public HotelService(IUnitOfWork unit, IMapper mapper)
    {
        _unit = unit;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HotelDto>> GetHotels(HotelFilterDto filter)
    {
        var hotels = await _unit.Hotels.GetAllAsync();

        var query = hotels.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Country))
            query = query.Where(x => x.Country == filter.Country);

        if (!string.IsNullOrWhiteSpace(filter.City))
            query = query.Where(x => x.City == filter.City);

        if (filter.Rating.HasValue)
            query = query.Where(x => x.Rating == filter.Rating);

        return _mapper.Map<IEnumerable<HotelDto>>(query);
    }

    public async Task<HotelDto?> GetHotelById(Guid id)
    {
        var hotel = await _unit.Hotels.GetByIdAsync(id);

        if (hotel == null)
            return null;

        return _mapper.Map<HotelDto>(hotel);
    }

    public async Task<HotelDto> CreateHotel(CreateHotelDto dto)
    {
        var hotel = _mapper.Map<Hotel>(dto);

        hotel.Id = Guid.NewGuid();

        await _unit.Hotels.AddAsync(hotel);

        await _unit.SaveAsync();

        return _mapper.Map<HotelDto>(hotel);
    }

    public async Task<HotelDto?> UpdateHotel(Guid id, UpdateHotelDto dto)
    {
        var hotel = await _unit.Hotels.GetByIdAsync(id);

        if (hotel == null)
            return null;

        hotel.Name = dto.Name;
        hotel.Address = dto.Address;
        hotel.Rating = (byte)dto.Rating;

        _unit.Hotels.Update(hotel);

        await _unit.SaveAsync();

        return _mapper.Map<HotelDto>(hotel);
    }

    public async Task<bool> DeleteHotel(Guid id)
    {
        var hotel = await _unit.Hotels.GetByIdAsync(id);

        if (hotel == null)
            return false;

        if (hotel.Rooms.Any())
            throw new Exception("Hotel has rooms and cannot be deleted");

        _unit.Hotels.Delete(hotel);

        await _unit.SaveAsync();

        return true;
    }
}