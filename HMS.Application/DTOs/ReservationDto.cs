namespace HMS.Application.DTOs
{
    public class ReservationDto
    {
        public Guid Id { get; set; }

        public Guid GuestId { get; set; }

        public DateTime CheckinDate { get; set; }

        public DateTime CheckoutDate { get; set; }

        public List<Guid> RoomIds { get; set; } = new();
    }
}