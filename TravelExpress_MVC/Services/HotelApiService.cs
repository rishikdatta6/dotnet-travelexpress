using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TravelExpress.Models;
using System.Linq;

namespace TravelExpress.Services
{
    public class HotelApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "http://localhost:3000/hotels";

        public HotelApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<HotelApi>> GetHotelsAsync()
        {
            var response = await _httpClient.GetAsync(_apiUrl);
            Console.WriteLine($"Status: {response.StatusCode}");


            if (!response.IsSuccessStatusCode)
            {
                return new List<HotelApi>();
            }

            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("API Response: " + content);

            // Deserialize into HotelApiResponse
            try
            {
                var hotels = JsonSerializer.Deserialize<List<HotelApi>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return hotels  ?? new List<HotelApi>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return new List<HotelApi>();
            }
        }

        public async Task<HotelApi> GetHotelByIdAsync(int id)
        {
            var hotels = await GetHotelsAsync();
            return hotels?.FirstOrDefault(h => h.HotelId == id);
        }

    }
}
