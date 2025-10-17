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

        public List<string> Cities { get; set; } = new();   // includes DriverCity first
        public string? Destination { get; set; }

        // to-do ask gpt about the syntax of this line
        public string DisplayRoute =>
            (Cities?.Count > 0 ? string.Join(", ", Cities) : "") + " \u2192 " + (Destination ?? "");
    }
}
