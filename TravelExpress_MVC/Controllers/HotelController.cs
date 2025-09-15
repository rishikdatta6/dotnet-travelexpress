using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TravelExpress.Models;
using TravelExpress.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace TravelExpress.Controllers
{
    public class HotelController : Controller
    {
        private readonly HotelApiService _hotelService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HotelController(HotelApiService hotelService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _hotelService = hotelService;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        

        //  Step 1: Show hotel list to all logged-in users 
        //[Authorize]
        public async Task<IActionResult> Index()
        {
            var hotels = await _hotelService.GetHotelsAsync();
            return View(hotels); // model: IEnumerable<HotelApi>
        }

        //  Step 2: Only Users can perform a hotel search
        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> Search(string location, DateTime checkIn, DateTime checkOut)
        {
            if (string.IsNullOrWhiteSpace(location) || checkIn >= checkOut || checkIn < DateTime.Today)
            {
                TempData["Error"] = "Please enter a valid location and date range.";
                return RedirectToAction("Index");
            }

            // Absolute protection: Prevent Admin if role was leaked
            if (!User.IsInRole("User"))
            {
                return Forbid(); // Or RedirectToAction("AccessDenied", "Home")
            }

            var hotels = await _hotelService.GetHotelsAsync();

            var filtered = hotels
                .Where(h => h.Location.Contains(location, StringComparison.OrdinalIgnoreCase))
                .ToList();

            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.Location = location;

            return View("SearchResults", filtered);
        }


        //  Step 3: Role-based behavior for hotel detail access
        public async Task<IActionResult> Details(int id)
        {
            var hotelApi = await _hotelService.GetHotelByIdAsync(id);
            if (hotelApi == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Room", new { hotelId = id });
            }

            var hotel = new Hotel
            {
                HotelId = hotelApi.HotelId,
                Name = hotelApi.Name,
                Location = hotelApi.Location
            };

            return View("Details", hotel);
        }
    }
}
