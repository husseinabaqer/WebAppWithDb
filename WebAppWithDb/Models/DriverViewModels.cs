using System.ComponentModel.DataAnnotations;


namespace WebAppWithDb.Models
{
    public class DriverOfferForm
    {

        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required] public string DriverCity { get; set; } = string.Empty;
        [Required] public string Destination { get; set; } = string.Empty;

        [Required] public string CarBrand { get; set; } = string.Empty;
        [Required] public string CarType { get; set; } = string.Empty;
        [Required] public string CarModel { get; set; } = string.Empty;

        [Range(3, 14)] public int CarSeats { get; set; }
        [Required] public string CarNumber { get; set; } = string.Empty;

        [Required] public string GenderPolicy { get; set; } = "Both"; // Both, MaleOnly ,FemaleOnly
        public string? OfferDescription { get; set; }

        //for multiple covered cities
        public string? CoveredCitiesCsv { get; set; }
    }

    public class DriverDashboardVM
    {
        public int DriverId { get; set; }
        public string FullName { get; set; } = string.Empty;

        public string? DriverCity { get; set; }
        public string? Destination { get; set; }

        public int CarSeats { get; set; }
        public int AvailableSeats { get; set; }

        public int PendingCount { get; set; }
        public int AcceptedCount { get; set; }

        public List<string> Cities { get; set; } = new();
    }

}

