using Microsoft.EntityFrameworkCore;
using TravelExpress.Models;

namespace TravelExpress.Services
{
    public class HotelService
    {
        private readonly AppDbContext _context;

        public HotelService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Hotel>> GetHotelsAsync()
        {
            return await _context.Hotels.ToListAsync();
        }

        public async Task<Hotel?> GetHotelByIdAsync(int id)
        {
            return await _context.Hotels
                .FirstOrDefaultAsync(h => h.HotelId == id);
        }
    }
}