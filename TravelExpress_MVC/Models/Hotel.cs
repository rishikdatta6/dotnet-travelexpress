using System.ComponentModel.DataAnnotations;

namespace TravelExpress.Models
{
    public class Hotel
    {
        public int HotelId { get; set; }


       
        public string Name { get; set; }


        public string Location { get;set; }

        
        //public ICollection<Room> Rooms { get; set; }
    }
}
