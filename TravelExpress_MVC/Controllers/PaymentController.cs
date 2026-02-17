using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelExpress.Models;
using TravelExpress.ViewModels;
using TravelExpress.Services;
namespace TravelExpress.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HotelApiService _hotelService;


        public PaymentController(AppDbContext context, UserManager<ApplicationUser> userManager, HotelApiService hotelService)
        {
            _context = context;
            _userManager = userManager;
            _hotelService = hotelService;
        }

        // ✅ General Index (Not used directly in navbar but can list all based on role)
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var payments = _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Room)
                .AsQueryable();

            if (!isAdmin)
            {
                payments = payments.Where(p => p.Booking.UserId == user.Id);
            }

            var paymentList = await payments.ToListAsync();
            return View(paymentList);
        }

        // ✅ GET: Payment/Create
        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> Create(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                return NotFound();

            ViewBag.PaymentMethods = new List<string> { "Credit Card", "UPI", "Net Banking", "Cash" };

            var payment = new Payment
            {
                BookingId = bookingId,
                HotelId = booking.Room.HotelId,
                Amount = booking.TotalAmount,
                PaymentDate = DateTime.Today
            };

            return View(payment);
        }

        // ✅ POST: Payment/Create
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int bookingId, string paymentMethod)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            // Get the booking along with any existing payment
            var booking = await _context.Bookings
                .Include(b => b.Payment)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == user.Id);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("MyBookings");
            }

            // ✅ Check if a payment already exists
            if (booking.Payment != null)
            {
                TempData["Info"] = "Payment already completed for this booking.";
                return RedirectToAction("MyPayments");
            }

            // ✅ Create new payment only if none exists
            var payment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = booking.TotalAmount,
                PaymentDate = DateTime.Now,
                PaymentMethod = paymentMethod // Assuming your model has this field
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment completed successfully!";
            return RedirectToAction("MyPayments");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PaymentSummary(int paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Room)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null || payment.Booking == null || payment.Booking.Room == null)
            {
                return NotFound();
            }

            // ✅ Get hotel name via API instead of EF Core
            var hotel = await _hotelService.GetHotelByIdAsync(payment.Booking.HotelId);

            var paymentViewModel = new PaymentWithHotelViewModel
            {
                PaymentId = payment.PaymentId,
                HotelName = hotel?.Name ?? "Unknown Hotel",
                RoomNumber = payment.Booking.Room.RoomNumber,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate
            };

            return View("PaymentSummary", paymentViewModel);
        }

        // ✅ Admin-only view
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            var payments = await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Room) // ✅ Required for Room Number
                .Include(p => p.Booking.User)
                .ToListAsync();

            return View(payments);
        }


        // ✅ User-only view (for navbar link)


        [Authorize]
        public async Task<IActionResult> MyPayments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var payments = await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Room)
                .Where(p => isAdmin || p.Booking.UserId == user.Id)
                .ToListAsync();

            var viewModelList = new List<PaymentWithHotelViewModel>();

            foreach (var payment in payments)
            {
                var hotel = await _hotelService.GetHotelByIdAsync(payment.Booking.HotelId);

                viewModelList.Add(new PaymentWithHotelViewModel
                {
                    PaymentId = payment.PaymentId,
                    HotelName = hotel?.Name ?? "Unknown Hotel",
                    RoomNumber = payment.Booking.Room.RoomNumber,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentDate = payment.PaymentDate
                });
            }

            return View(viewModelList);
        }


    }
}
