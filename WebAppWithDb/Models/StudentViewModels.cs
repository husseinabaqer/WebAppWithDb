namespace WebAppWithDb.Models
{
    public class StudentRideVM
    {
        public bool HasRide { get; set; }
        public string? DriverName { get; set; }
        public string? Route { get; set; }
        public string? Car { get; set; }
        public string? GenderPolicy { get; set; }
        public int? SeatsLeft { get; set; }
    }
}
