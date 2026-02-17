using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelExpress.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }  // <-- REQUIRED
  // <-- REQUIRED

        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }
         // ✅ Make sure this is in the class
        public decimal TotalAmount { get; set; }    // ✅
 
        public ApplicationUser User { get; set; }
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Now;  // <-- OPTIONAL BUT RECOMMENDED

        // <-- REQUIRED for Payments
        [Required]
        public int HotelId { get; set; }  // Foreign key to HotelApi
        public Payment? Payment { get; set; }
    }
}
