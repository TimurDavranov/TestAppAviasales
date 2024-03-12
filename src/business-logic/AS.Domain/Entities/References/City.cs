using AS.Core.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.Domain.Entities.References
{
    [Table("cities", Schema = "references")]
    public class City : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public State State { get; set; }
        public ICollection<Airport> Airports { get; set; }
    }
}
