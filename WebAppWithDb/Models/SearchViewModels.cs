namespace WebAppWithDb.Models
{
    public class SearchFiltersVM
    {
        public string? City { get; set; }
        public string? Destination { get; set; }
        public string? GenderPolicy { get; set; } // Both/MaleOnly/FemaleOnly
        public string? CarType { get; set; }
    }

    public class SearchResultItemVM
    {
        public int DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Car { get; set; } = string.Empty;
        public string GenderPolicy { get; set; } = "Both";
        public int AvailableSeats { get; set; }
        public string PhoneNumber { get; set; } = "";//filled from Identity user
    }
    

    public class SearchResultsVM
    {
        public SearchFiltersVM Filters { get; set; } = new();
        public List<SearchResultItemVM> Results { get; set; } = new();
    }


}
