using AS.Core.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.Domain.Entities.References
{
    [Table("airports", Schema = "references")]
    public class Airport : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public City City { get; set; }
    }


    [Table("avia_companies", Schema = "references")]
    public class AviaCompany : BaseEntity<Guid>
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
