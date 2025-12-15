
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BenzenyMain.Domain.Entity
{
    public class City
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Guid RegionId { get; set; }

        [ForeignKey(nameof(RegionId))]
        public Region Region { get; set; } = null!;
        public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    }
}
