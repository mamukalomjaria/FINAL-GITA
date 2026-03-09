using HMS.Core.Entities;

public class Reservation
{
    public Guid Id { get; set; }

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public Guid GuestId { get; set; }

    public Guest Guest { get; set; }

    public ICollection<ReservationRoom> ReservationRooms { get; set; }
}