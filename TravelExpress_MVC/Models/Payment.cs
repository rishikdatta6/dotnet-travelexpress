using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelExpress.Models
{
    public class Payment
    {
        public int PaymentId{get; set;} 

        [Required]
        public DateTime PaymentDate { get; set; }

        public string PaymentMethod {  get; set; }

        [Required]
        public decimal Amount {  get; set; }

        public int BookingId { get; set; }
        public int HotelId {  get; set; }

        public Booking? Booking { get; set; }
    }
}
