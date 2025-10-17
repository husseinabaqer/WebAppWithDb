using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;


namespace WebAppWithDb.Data.Tables
{
    public class Driver
    {
        // driver info on the right side in the sign up
        [Key]
        public int DriverId { get; set; }

        [Required]
        public string? FullName { get; set; }
        public string?  DriverCity { get; set; }
        public string? Destination { get; set; }


        // car info on the left side, this happen if lets say the user chose driver not user when first sign up
        [Required]
        public string CarBrand { get; set; } = string.Empty; 
        [Required]
        public string CarType { get; set; } = string.Empty;
        [Required]
        public string CarModel { get; set; } = string.Empty;

        [Range(3, 14)]
        public int CarSeats { get; set; }

        [Required, MaxLength(10)]
        public string CarNumber { get; set; } = string.Empty;

        public int AvailableSeats { get; set; }

        [MaxLength(500)]
        public string? OfferDescription { get; set; }

        [Required, MaxLength(20)]
        public string GenderPolicy { get; set; } = "Both"; // Both.MaleOnly.FemaleOnly

        public bool IsActive { get; set; } = true;

        [Required]
        public string UserId { get; set; } = string.Empty;


        //will be used with .include(d => d.CoveredCities)
        public virtual ICollection<CoveredCity?>? CoveredCities { get; set; } = new List<CoveredCity>();
        public virtual ICollection<Request>? Requests { get; set; } = new List<Request>();

    }
}
