using System.ComponentModel.DataAnnotations;

namespace HMS.Core.Entities
{
    public class Reservation
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CheckinDate { get; set; }

        public DateTime CheckoutDate { get; set; }

        public string GuestId { get; set; }
        public ApplicationUser Guest { get; set; }

        public ICollection<ReservationRoom> ReservationRooms { get; set; }
    }
}