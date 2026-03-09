using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Core.Entities
{
    public class Hotel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Range(1, 5)]
        public byte Rating { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        public string ManagerId { get; set; }
        public ApplicationUser Manager { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }
}