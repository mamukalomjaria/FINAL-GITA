using System.ComponentModel.DataAnnotations;

namespace HMS.Core.Entities
{
    public class Hotel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string Country { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public ICollection<ApplicationUser> Managers { get; set; } = new List<ApplicationUser>();

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}