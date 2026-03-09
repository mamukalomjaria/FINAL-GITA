using HMS.Application.DTOs;

namespace HMS.Application.Services
{
    public interface IManagerService
    {
        Task<ManagerDto> CreateManager(Guid hotelId, CreateManagerDto dto);
        Task<bool> UpdateManager(Guid managerId, UpdateManagerDto dto);
        Task<bool> DeleteManager(Guid managerId);
    }
}