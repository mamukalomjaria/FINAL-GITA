using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HMS.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(11)]
        [MinLength(11)]
        public string PersonalNumber { get; set; }

        public ICollection<Hotel> ManagedHotels { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}