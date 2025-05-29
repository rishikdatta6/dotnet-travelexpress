using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelExpress.Models;

namespace TravelExpress.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;   
        
        public BookingController(AppDbContext context)
        {
            _context = context;
        }
         
        public async Task<IActionResult>Index(int roomId)
        {
            var room= await _context.Rooms.Include(r=>r.Bookings).FirstOrDefaultAsync(r=>r.RoomId==roomId);
            if(room==null)
            {
                return NotFound();
            }

            var bookings=await _context.Bookings.Where(b=>b.RoomId==roomId).OrderBy(b=>b.checkIn).ToListAsync();
            ViewBag.RoomId = roomId;
            ViewBag.RoomNumber=room.RoomNumber;
            return View(bookings);
        }
        public async Task<IActionResult> Details(int id)
        {
            var booking= await _context.Bookings.Include(b=>b.Room).Include(b=>b.Payment).FirstOrDefaultAsync(b=>b.BookingId==id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }
        [HttpGet]
        public async Task<IActionResult> Create(int roomId)
        {
            var room= await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                return NotFound();
            }
              var booking = new Booking
            {
                RoomId = roomId,
                HotelId = room.HotelId,
                checkIn=DateTime.Today,
                checkOut=DateTime.Today.AddDays(1)
            };

            return View(booking);

        }

        [HttpPost]
        public async Task<IActionResult> Create(Booking booking)
        {
            if(booking.checkIn >= booking.checkOut)
            {
                ModelState.AddModelError("", "Check-out must be after check-in");
                return View(booking);
            }

            var isRoomAvailable = !_context.Bookings.Where(b => b.RoomId == booking.RoomId).Any(b => booking.checkIn < b.checkOut && b.checkIn < booking.checkOut);

            if (!isRoomAvailable)
            {
                ModelState.AddModelError("", "The room is not available for the selected dates");
            }

               if (ModelState.IsValid)
            {
                
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = booking.BookingId });
            }


            return View(booking);
        }
        [HttpGet]
        public JsonResult CheckAvailability(int roomId, DateTime checkIn, DateTime checkOut)
        {
            // Check if any existing booking for the room overlaps with requested period
            var isBooked = _context.Bookings
                .Any(b => b.RoomId == roomId &&
                    (checkIn < b.checkOut && checkOut > b.checkIn));

            // Return JSON indicating availability (true if no overlap)
            return Json(new { available = !isBooked });
        }
    }
}
