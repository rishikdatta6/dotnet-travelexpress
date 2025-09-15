// Models/ViewModels/PaymentViewModel.cs
namespace TravelExpress.ViewModels
{
    public class PaymentWithHotelViewModel
    {
        public int PaymentId { get; set; }
        public string HotelName { get; set; }
        public string RoomNumber { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
