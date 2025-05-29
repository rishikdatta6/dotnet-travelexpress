using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelExpress.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public DateTime checkIn { get; set; }

        [Required]
        public DateTime checkOut { get; set; }

        public int HotelId { get; set; }     
        public int RoomId { get; set; }

        public Room? Room { get; set; }
        public Payment? Payment { get; set; }

    }
}
