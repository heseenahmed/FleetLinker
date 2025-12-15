
using BenzenyMain.Domain.Entity.Dto.Branch;

namespace BenzenyMain.Domain.Entity.Dto.Region
{
    public class RegionForListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public List<BranchForListDto>? Branches { get; set; }
    }
}
