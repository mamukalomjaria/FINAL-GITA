using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs
{
    public class CreateHotelDto
    {
        [Required]
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
    }
}