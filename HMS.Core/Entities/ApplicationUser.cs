using Microsoft.AspNetCore.Identity;

namespace HMS.Core.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string PersonalNumber { get; set; } = string.Empty;

        public Guid? HotelId { get; set; }

        public Hotel? Hotel { get; set; }
    }
}