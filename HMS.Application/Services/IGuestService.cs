using HMS.Application.DTOs;

namespace HMS.Application.Services
{
    public interface IGuestService
    {
        Task<GuestDto> CreateGuest(CreateGuestDto dto);

        Task<GuestDto?> GetGuest(Guid id);

        Task<bool> UpdateGuest(Guid id, UpdateGuestDto dto);

        Task<bool> DeleteGuest(Guid id);
    }
}