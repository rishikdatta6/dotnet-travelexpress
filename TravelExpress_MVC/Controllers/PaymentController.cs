using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelExpress.Models;

namespace TravelExpress.Controllers
{
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            var payment=await _context.Payments.Include(p=>p.Booking).FirstOrDefaultAsync(p=>p.PaymentId==id);
            if (payment==null)
            {
                return NotFound();
            }
            return View(payment);
        }

        public IActionResult Create(int bookingId)
        {
            var booking = _context.Bookings.Include(b => b.Room)
                .FirstOrDefault(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return NotFound();
            }
            var payment = new Payment
            {
                BookingId = bookingId,
                HotelId = booking.HotelId,
                Amount = booking.Room?.PricePerNight ?? 0,
                PaymentDate = DateTime.Now
            };

            ViewBag.PaymentMethods = new List<string>
            {
                "UPI", "Credit Card", "Debit Card", "Net Banking"
            };
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Booking", new { id = payment.BookingId });
            }

            return View(payment);
        }
    }
}
