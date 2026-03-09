public class CreateReservationDto
{
    public DateTime CheckinDate { get; set; }

    public DateTime CheckoutDate { get; set; }

    public string GuestId { get; set; }

    public List<Guid> RoomIds { get; set; }
}