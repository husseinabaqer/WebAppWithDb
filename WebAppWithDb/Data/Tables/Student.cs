using System.ComponentModel.DataAnnotations;

namespace WebAppWithDb.Data.Tables
{
    public class Student //this when the user choose student to sign up---sign up form
    {
        [Key]
        public int StudentId { get; set; }

        [Required, MaxLength(100)]
        public string? FullName { get; set; } = string.Empty;

        public string? Gender { get; set; }
        public string? Stage { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? Destination { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ICollection<Request>? Requests { get; set; }
    }
}
