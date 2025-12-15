
using Benzeny.Infra.Data;
using Benzeny.Infra.Repository;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.IRepository;

namespace BenzenyMain.Infra.Repository
{
    public class CarTypeRepository : BaseRepository<CarType> , ICarTypeRepository
    {
        public CarTypeRepository(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
