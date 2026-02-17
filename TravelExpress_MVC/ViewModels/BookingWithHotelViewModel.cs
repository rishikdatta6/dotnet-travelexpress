// Models/ViewModels/BookingViewModel.cs
namespace TravelExpress.ViewModels
{
    public class BookingWithHotelViewModel
    {
        public int BookingId { get; set; }
        public string HotelName { get; set; }
        public string RoomType { get; set; }
        public string RoomNumber { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
