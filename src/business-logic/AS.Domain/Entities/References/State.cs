using AS.Core.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.Domain.Entities.References
{
    [Table("states", Schema = "references")]
    public class State : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Country Country { get; set; }
        public ICollection<City> Cities { get; set; }
    }
}
