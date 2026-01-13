using FleetLinker.Domain.Entity;
using FleetLinker.Domain.IRepository;
using FleetLinker.Infra.Data;

namespace FleetLinker.Infra.Repository
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
