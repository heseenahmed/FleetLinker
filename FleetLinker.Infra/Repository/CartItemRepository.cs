using FleetLinker.Domain.Entity;
using FleetLinker.Domain.IRepository;
using FleetLinker.Infra.Data;

namespace FleetLinker.Infra.Repository
{
    public class CartItemRepository : BaseRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
