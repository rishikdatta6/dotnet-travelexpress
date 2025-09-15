using System;
using System.ComponentModel.DataAnnotations;

namespace TravelExpress.Models
{
    public class HotelSearchViewModel
    {
        [Required]
        public string Location { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }
    }
}
