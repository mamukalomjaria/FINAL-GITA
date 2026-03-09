using HMS.Core.Entities;

public class Reservation
{
    public Guid Id { get; set; }

    public DateTime CheckinDate { get; set; }

    public DateTime CheckoutDate { get; set; }

    public Guid GuestId { get; set; }

    public Guest Guest { get; set; }

    public ICollection<ReservationRoom> ReservationRooms { get; set; }
}