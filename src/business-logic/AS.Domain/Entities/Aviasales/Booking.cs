using AS.Core.Enums;
using AS.Core.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.Domain.Entities.Aviasales
{
    [Table("bookings", Schema = "aviasales")]
    public class Booking : BaseEntity<Guid>
    {
        public required Guid UserId { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime ExpiresDate { get; set; }
        public Guid TicketId { get; set; }
        public BookingStatus Status { get; set; }
        public string? Error { get; set; }
    }
}
