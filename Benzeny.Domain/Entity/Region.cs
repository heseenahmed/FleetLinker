
using Benzeny.Domain.Entity;
using System.ComponentModel.DataAnnotations;

namespace BenzenyMain.Domain.Entity
{
    public class Region
    {
        [Key]
        public Guid Id { get; set; } = new Guid();
        public string Title { get; set; } = null!;
        public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();
    }
}
