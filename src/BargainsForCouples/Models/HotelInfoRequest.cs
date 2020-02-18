using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BargainsForCouples.Models
{
    public class HotelInfoRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int Nights { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int DestinationId { get; set; }
    }
}