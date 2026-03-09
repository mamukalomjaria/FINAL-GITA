using HMS.Application.DTOs;
using HMS.Core.Entities;
using HMS.Infrastructure.UnitOfWork;

namespace HMS.Application.Services
{
    public class GuestService : IGuestService
    {
        private readonly IUnitOfWork _unit;

        public GuestService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<GuestDto> CreateGuest(CreateGuestDto dto)
        {
            var guests = await _unit.Guests.GetAllAsync();

            if (guests.Any(g => g.PersonalNumber == dto.PersonalNumber))
                throw new Exception("Personal number already exists");

            if (guests.Any(g => g.PhoneNumber == dto.PhoneNumber))
                throw new Exception("Phone number already exists");

            var guest = new Guest
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PersonalNumber = dto.PersonalNumber,
                PhoneNumber = dto.PhoneNumber
            };

            await _unit.Guests.AddAsync(guest);
            await _unit.SaveAsync();

            return new GuestDto
            {
                Id = guest.Id,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                PersonalNumber = guest.PersonalNumber,
                PhoneNumber = guest.PhoneNumber
            };
        }

        public async Task<GuestDto?> GetGuest(Guid id)
        {
            var guest = await _unit.Guests.GetByIdAsync(id);

            if (guest == null)
                return null;

            return new GuestDto
            {
                Id = guest.Id,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                PersonalNumber = guest.PersonalNumber,
                PhoneNumber = guest.PhoneNumber
            };
        }

        public async Task<bool> UpdateGuest(Guid id, UpdateGuestDto dto)
        {
            var guest = await _unit.Guests.GetByIdAsync(id);

            if (guest == null)
                return false;

            var guests = await _unit.Guests.GetAllAsync();

            if (guests.Any(g => g.PhoneNumber == dto.PhoneNumber && g.Id != id))
                throw new Exception("Phone number already exists");

            guest.FirstName = dto.FirstName;
            guest.LastName = dto.LastName;
            guest.PhoneNumber = dto.PhoneNumber;

            _unit.Guests.Update(guest);

            await _unit.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteGuest(Guid id)
        {
            var guest = await _unit.Guests.GetByIdAsync(id);

            if (guest == null)
                return false;

            if (guest.Reservations.Any(r => r.CheckOutDate >= DateTime.Today))
                throw new Exception("Guest has active or future reservations");

            _unit.Guests.Delete(guest);

            await _unit.SaveAsync();

            return true;
        }
    }
}