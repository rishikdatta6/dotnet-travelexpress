using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelExpress.Models
{
    public class Room
    {

        public int RoomId { get; set; }

        [Required]
        public string RoomNumber { get; set; }

        public string RoomType{ get; set; }


        [Required]
        public decimal PricePerNight {  get; set; }
        public int HotelId { get; set; }

    

        public ICollection<Booking>? Bookings { get; set; }

    }
}
