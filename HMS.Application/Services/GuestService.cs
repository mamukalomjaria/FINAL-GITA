using HMS.Application.DTOs;
using HMS.Core.Entities;
using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services
{
    public class GuestService : IGuestService
    {
        private readonly ApplicationDbContext _context;

        public GuestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GuestDto> CreateGuest(CreateGuestDto dto)
        {
            var personalExists = await _context.Guests
                .AnyAsync(g => g.PersonalNumber == dto.PersonalNumber);

            if (personalExists)
                throw new Exception("Personal number already exists");

            var phoneExists = await _context.Guests
                .AnyAsync(g => g.PhoneNumber == dto.PhoneNumber);

            if (phoneExists)
                throw new Exception("Phone number already exists");

            var guest = new Guest
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PersonalNumber = dto.PersonalNumber,
                PhoneNumber = dto.PhoneNumber
            };

            _context.Guests.Add(guest);

            await _context.SaveChangesAsync();

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
            var guest = await _context.Guests.FindAsync(id);

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
            var guest = await _context.Guests.FindAsync(id);

            if (guest == null)
                return false;

            guest.FirstName = dto.FirstName;
            guest.LastName = dto.LastName;
            guest.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteGuest(Guid id)
        {
            var guest = await _context.Guests
                .Include(g => g.Reservations)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (guest == null)
                return false;

            if (guest.Reservations.Any(r => r.CheckoutDate >= DateTime.Today))
                throw new Exception("Guest has active reservations");

            _context.Guests.Remove(guest);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}