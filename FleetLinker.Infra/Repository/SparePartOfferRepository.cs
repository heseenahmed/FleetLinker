using FleetLinker.Domain.Entity;
using FleetLinker.Domain.IRepository;
using FleetLinker.Infra.Data;

namespace FleetLinker.Infra.Repository
{
    public class SparePartOfferRepository : BaseRepository<SparePartOffer>, ISparePartOfferRepository
    {
        public SparePartOfferRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
