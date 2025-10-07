using System.ComponentModel.DataAnnotations;

namespace WebAppWithDb.Data.Tables
{
    public class CoveredCity
    {
        [Key]
        public int CityId { get; set; }

        [Required]
        public string City { get; set; } = string.Empty;

        public int DriverId { get; set; }
        public virtual Driver? Driver { get; set; }

    }
}
