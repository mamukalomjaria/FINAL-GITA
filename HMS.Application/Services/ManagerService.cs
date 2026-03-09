using HMS.Application.DTOs;
using HMS.Core.Entities;
using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services
{
    public class ManagerService : IManagerService
    {
        private readonly ApplicationDbContext _context;

        public ManagerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ManagerDto> CreateManager(Guid hotelId, CreateManagerDto dto)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (emailExists)
                throw new Exception("Email already exists");

            var manager = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                UserName = dto.Email
            };

            _context.Users.Add(manager);

            await _context.SaveChangesAsync();

            return new ManagerDto
            {
                Id = manager.Id,
                Email = manager.Email,
                PhoneNumber = manager.PhoneNumber
            };
        }

        public async Task<bool> UpdateManager(Guid managerId, UpdateManagerDto dto)
        {
            var manager = await _context.Users.FindAsync(managerId.ToString());

            if (manager == null)
                return false;

            manager.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteManager(Guid managerId)
        {
            var manager = await _context.Users.FindAsync(managerId.ToString());

            if (manager == null)
                return false;

            _context.Users.Remove(manager);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}