using FleetLinker.Domain.Entity;
using FleetLinker.Domain.IRepository;
using FleetLinker.Infra.Data;

namespace FleetLinker.Infra.Repository
{
    public class EquipmentSparePartRepository : BaseRepository<EquipmentSparePart>, IEquipmentSparePartRepository
    {
        public EquipmentSparePartRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
