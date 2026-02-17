using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravelExpress.Models;
using TravelExpress.Services;

namespace TravelExpress.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HotelApiService _hotelApiService;

       


        public RoomController(AppDbContext context, UserManager<ApplicationUser> userManager, HotelApiService hotelService)
        {
            _context = context;
            _userManager = userManager;
            _hotelApiService = hotelService;
        }

        // 🟢 Admin-only: View all rooms
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int hotelId)
        {
            ViewBag.HotelId = hotelId;

            var rooms = await _context.Rooms
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();

            return View(rooms);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            // 🔄 Fetch hotel via HotelApiService
            var hotel = await _hotelApiService.GetHotelByIdAsync(room.HotelId);

            // 👇 Pass hotel info using ViewBag
            ViewBag.HotelName = hotel?.Name ?? "Unknown";
            ViewBag.HotelLocation = hotel?.Location ?? "Unknown";

            return View(room);
        }



        // 🟢 Admin-only: Create room (GET)
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int hotelId)
        {
            return View(new Room
            {
                HotelId = hotelId
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
            if (!ModelState.IsValid)
            {
                // Log errors to console
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        Console.WriteLine($"Validation Error - {key}: {error.ErrorMessage}");
                    }
                }

                TempData["Error"] = "Validation failed. Check field values.";
                return View(room);
            }

            switch (room.RoomType)
            {
                case RoomType.Single: room.PricePerNight = 3500; break;
                case RoomType.Double: room.PricePerNight = 5000; break;
                case RoomType.Deluxe: room.PricePerNight = 7500; break;
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Room created successfully!";
            return RedirectToAction(nameof(Index), new { hotelId = room.HotelId });
        }


        // 🟢 Admin-only: Edit room (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        // 🟢 Admin-only: Edit room (POST)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.RoomId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(room);

            // Recalculate price (in case RoomType was not posted properly)
            switch (room.RoomType)
            {
                case RoomType.Single: room.PricePerNight = 3500; break;
                case RoomType.Double: room.PricePerNight = 5000; break;
                case RoomType.Deluxe: room.PricePerNight = 7500; break;
            }

            _context.Update(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { hotelId = room.HotelId });
        }


        // 🟢 Admin-only: Delete room (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                int hotelId = room.HotelId; // ✅ Save hotelId before deleting
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Room deleted successfully!";
                return RedirectToAction(nameof(Index), new { hotelId = hotelId }); // ✅ Redirect properly
            }

            return NotFound();
        }


        // 🔵 User-only: View available rooms by hotel and date
        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> UserRoomDetails(int hotelId, DateTime checkIn, DateTime checkOut)
        {
            // ✅ Ensure the check-in date is earlier than check-out date
            if (checkIn >= checkOut)
            {
                ModelState.AddModelError("", "Check-in date must be earlier than check-out date.");
                return RedirectToAction("Index", "Hotel");
            }

            // ⛔ Prevent booking for past check-in date
            if (checkIn < DateTime.Today)
            {
                TempData["Error"] = "Check-in date cannot be in the past.";
                return RedirectToAction("Index", "Hotel");
            }

            var hotel = await _hotelApiService.GetHotelByIdAsync(hotelId);
            ViewBag.HotelName = hotel?.Name ?? "Unknown";
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;

            var rooms = await _context.Rooms
                .Where(r => r.HotelId == hotelId)
                .Include(r => r.Bookings) // Include Bookings for room availability check
                .ToListAsync();

            // Filter available rooms
            var availableRooms = rooms
    .Where(r => !r.Bookings.Any(b => checkIn < b.CheckOut && checkOut > b.CheckIn || checkIn == b.CheckOut))
    .ToList();


            return View(availableRooms);
        }

    }
}
