using FleetLinker.Domain.Entity;
using FleetLinker.Domain.IRepository;
using FleetLinker.Infra.Data;

namespace FleetLinker.Infra.Repository
{
    public class EquipmentRequestRepository : BaseRepository<EquipmentRequest>, IEquipmentRequestRepository
    {
        public EquipmentRequestRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
