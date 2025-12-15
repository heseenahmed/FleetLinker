
using Benzeny.Infra.Data;
using Benzeny.Infra.Repository;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.IRepository;

namespace BenzenyMain.Infra.Repository
{
    public class CarBrandRepository : BaseRepository<CarBrand> , ICarBrandRepository
    {
        public CarBrandRepository(ApplicationDbContext _context):base(_context)
        {
            
        }
    }
}
