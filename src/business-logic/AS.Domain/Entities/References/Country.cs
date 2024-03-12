using AS.Core.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.Domain.Entities.References
{
    [Table("countries", Schema = "references")]
    public class Country : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string NumericCode { get; set; }
        public string ISOCode2 { get; set; }
        public string ISOCode3 { get; set; }
        public ICollection<State> States { get; set; }
    }
}
