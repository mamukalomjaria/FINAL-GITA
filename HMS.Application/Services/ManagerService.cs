using HMS.Application.DTOs;
using HMS.Core.Entities;
using HMS.Infrastructure.UnitOfWork;

namespace HMS.Application.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IUnitOfWork _unit;

        public ManagerService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<ManagerDto> CreateManager(Guid hotelId, CreateManagerDto dto)
        {
            var hotels = await _unit.Hotels.GetAllAsync();

            if (!hotels.Any(h => h.Id == hotelId))
                throw new Exception("Hotel not found");

            var managers = await _unit.Users.GetAllAsync();

            if (managers.Any(u => u.Email == dto.Email))
                throw new Exception("Email already exists");

            if (managers.Any(u => u.PersonalNumber == dto.PersonalNumber))
                throw new Exception("Personal number already exists");

            var manager = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PersonalNumber = dto.PersonalNumber,
                HotelId = hotelId
            };

            await _unit.Users.AddAsync(manager);
            await _unit.SaveAsync();

            return new ManagerDto
            {
                Id = manager.Id,
                Email = manager.Email,
                PhoneNumber = manager.PhoneNumber
            };
        }

        public async Task<bool> UpdateManager(Guid managerId, UpdateManagerDto dto)
        {
            var managers = await _unit.Users.GetAllAsync();

            var manager = managers.FirstOrDefault(m => m.Id == managerId);

            if (manager == null)
                return false;

            manager.PhoneNumber = dto.PhoneNumber;

            _unit.Users.Update(manager);

            await _unit.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteManager(Guid managerId)
        {
            var managers = await _unit.Users.GetAllAsync();

            var manager = managers.FirstOrDefault(m => m.Id == managerId);

            if (manager == null)
                return false;

            var managerCount = managers.Count(m => m.HotelId == manager.HotelId);

            if (managerCount <= 1)
                throw new Exception("Hotel must have at least one manager");

            _unit.Users.Delete(manager);

            await _unit.SaveAsync();

            return true;
        }
    }
}