using System.ComponentModel.DataAnnotations;

namespace WebAppWithDb.Data.Tables
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }

        public int DriverId { get; set; }
        public virtual Driver? Driver { get; set; }

        public int StudentId { get; set; } 
        public virtual Student? Student { get; set; }

        public string Status { get; set; } = "Pending"; // do nothing, but if accepted or rejected or removed this will change the seats available in the offer or post
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DecidedAt { get; set; }
    }
}
