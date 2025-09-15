using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravelExpress.Models;
using TravelExpress.ViewModels;
using TravelExpress.Services;

namespace TravelExpress.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HotelApiService _hotelService;

        public BookingController(AppDbContext context, UserManager<ApplicationUser> userManager, HotelApiService hotelService)
        {
            _context = context;
            _userManager = userManager;
            _hotelService = hotelService;
        }


        


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDirect(int roomId, DateTime checkIn, DateTime checkOut)
        {
            if (checkIn.Date < DateTime.Today || checkOut.Date <= checkIn.Date)
            {
                TempData["Error"] = "Invalid check-in or check-out dates.";
                return RedirectToAction("UserRoomDetails", "Room", new { roomId, checkIn, checkOut });
            }

            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
            {
                TempData["Error"] = "Room not found.";
                return RedirectToAction("UserRoomDetails", "Room", new { checkIn, checkOut });
            }

            var hotel = await _hotelService.GetHotelByIdAsync(room.HotelId);
            if (hotel == null)
            {
                TempData["Error"] = "Hotel information could not be retrieved.";
                return RedirectToAction("UserRoomDetails", "Room", new { checkIn, checkOut });
            }

            var user = await _userManager.GetUserAsync(User);

            // 🔒 Prevent double-booking by this or any user with proper date overlap check
            bool isAvailable = !room.Bookings.Any(b =>
                !(checkOut <= b.CheckIn || checkIn >= b.CheckOut)); // correct overlap logic

            if (!isAvailable)
            {
                TempData["Error"] = "This room is no longer available for the selected dates.";
                return RedirectToAction("UserRoomDetails", "Room", new { hotelId = room.HotelId, checkIn, checkOut });
            }

            // 🔁 Prevent exact same booking being inserted again (refresh/back button case)
            var existingBooking = await _context.Bookings.FirstOrDefaultAsync(b =>
                b.UserId == user.Id &&
                b.RoomId == roomId &&
                b.CheckIn == checkIn &&
                b.CheckOut == checkOut);

            if (existingBooking != null)
            {
                return RedirectToAction("BookingSummary", new { bookingId = existingBooking.BookingId });
            }

            // ✅ Proceed with booking inside transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var booking = new Booking
                {
                    RoomId = roomId,
                    UserId = user.Id,
                    HotelId = room.HotelId,
                    CheckIn = checkIn,
                    CheckOut = checkOut,
                    BookingDate = DateTime.Now,
                    TotalAmount = (checkOut - checkIn).Days * room.PricePerNight
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("BookingSummary", new { bookingId = booking.BookingId });
            }
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .ToListAsync();

            return View(bookings);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BookingSummary(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null || booking.UserId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            // ✅ Get hotel from HotelApiService instead of EF
            var hotel = await _hotelService.GetHotelByIdAsync(booking.Room.HotelId);

            var bookingViewModel = new BookingWithHotelViewModel
            {
                BookingId = booking.BookingId,
                HotelName = hotel?.Name ?? "Unknown",
                RoomType = booking.Room.RoomType.ToString(),
                RoomNumber = booking.Room.RoomNumber,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalAmount = booking.TotalAmount,
                BookingDate = booking.BookingDate
            };

            return View(bookingViewModel);
        }

        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // Get bookings with Room only (no Hotel included)
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => isAdmin || b.UserId == user.Id)
                .ToListAsync();

            var bookingViewModels = new List<BookingWithHotelViewModel>();

            foreach (var booking in bookings)
            {
                var hotel = await _hotelService.GetHotelByIdAsync(booking.Room.HotelId);

                bookingViewModels.Add(new BookingWithHotelViewModel
                {
                    BookingId = booking.BookingId,
                    HotelName = hotel?.Name ?? "Unknown Hotel",
                    RoomType = booking.Room.RoomType.ToString(),
                    RoomNumber = booking.Room.RoomNumber,
                    CheckIn = booking.CheckIn,
                    CheckOut = booking.CheckOut,
                    TotalAmount = booking.TotalAmount,
                    BookingDate = booking.BookingDate
                });
            }

            return View(bookingViewModels);
        }

    }
}

