
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelExpress.Models;


namespace TravelExpress.Controllers
{
    public class RoomController : Controller
    {
        private readonly AppDbContext _context;

        public RoomController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int hotelId)
        {
            var rooms = await _context.Rooms.Include(r => r.Bookings).Where(r => r.HotelId == hotelId).ToListAsync();
            ViewBag.HotelId = hotelId;
            return View(rooms);
        }


        public async Task<IActionResult> Details(int id)
        {
            var room =await _context.Rooms.FindAsync(id);
            if(room==null)
            {
                return NotFound();
            }

            return View(room);
        }


        public IActionResult Create(int hotelId)
        {
            var room=new Room { HotelId = hotelId };
            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult>Create(Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { hotelId = room.HotelId });
            }

            return View(room);
        }


        public async Task<IActionResult>Edit(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Room room)
        {
            if (ModelState.IsValid)
            {

                _context.Update(room);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { hotelId = room.HotelId });
            }
            return View(room);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if(room == null)
            {
                return NotFound();
            }
            var hotelId = room.HotelId;
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            
                return RedirectToAction("Index", new { hotelId = hotelId });
            
        }

    }
}
