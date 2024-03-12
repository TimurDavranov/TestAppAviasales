using AS.Core.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.Domain.Entities.Aviasales
{
    [Table("contracts", Schema = "aviasales")]
    public class Contract : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public Booking Booking { get; set; }
    }
}
