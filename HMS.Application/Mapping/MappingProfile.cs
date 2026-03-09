using AutoMapper;
using HMS.Application.DTOs;
using HMS.Core.Entities;

namespace HMS.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateHotelDto, Hotel>();
            CreateMap<Hotel, HotelDto>();
        }
    }
}