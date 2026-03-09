using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs
{
    public class CreateRoomDto
    {
        [Required]
        public string Name { get; set; }

        [Range(1, double.MaxValue)]
        public double Price { get; set; }
    }
}