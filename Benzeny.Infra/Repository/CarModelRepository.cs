
using Benzeny.Infra.Data;
using Benzeny.Infra.Repository;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.IRepository;

namespace BenzenyMain.Infra.Repository
{
    public class CarModelRepository : BaseRepository<CarModel>, ICarModelRepository
    {
        public CarModelRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            
        }
    }
}
