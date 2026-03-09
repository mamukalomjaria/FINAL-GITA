namespace HMS.Application.DTOs
{
    public class RoomDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public Guid HotelId { get; set; }
    }
}