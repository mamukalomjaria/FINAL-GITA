using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Core.Entities
{
    public class Room
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; }

        public ICollection<ReservationRoom> ReservationRooms { get; set; }
    }
}