using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TravelExpress.Services;


namespace TravelExpress.Controllers
{

    public class HotelController : Controller
    {
        private readonly HotelApiService _hotelService;

        public HotelController(HotelApiService hotelService)
        {
            _hotelService = hotelService;

        }
        public async Task<IActionResult> Index()
        {
            var hotels = await _hotelService.GetHotelsAsync();
            return View(hotels);
        }

      
        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
            {
                return NotFound();

            }
            ViewBag.Hotel = hotel;
            return View(hotel);
        }

    

    }
}