using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelExpress.Models
{
    public class Room
    {
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Room number is required.")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Room type is required.")]
        public RoomType? RoomType { get; set; }

        public decimal PricePerNight { get; set; }

        [Required]
        public int HotelId { get; set; }

        public Hotel? Hotel { get; set; }

        // ✅ Add this back to resolve the "Bookings missing" error
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
