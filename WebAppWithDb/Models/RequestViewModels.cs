using WebAppWithDb.Data.Tables;

namespace WebAppWithDb.Models
{
    public class RequestsInboxVM
    {
        public int DriverId { get; set; }
        public List<Request> Pending { get; set; } = new();
        public List<Request> Accepted { get; set; } = new();
        public List<Request> Rejected { get; set; } = new();
    }
}
