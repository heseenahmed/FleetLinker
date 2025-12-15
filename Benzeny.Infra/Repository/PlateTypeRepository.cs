
using Benzeny.Domain.Entity;
using Benzeny.Infra.Data;
using Benzeny.Infra.Repository;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.IRepository;

namespace BenzenyMain.Infra.Repository
{
    public class PlateTypeRepository : BaseRepository<PlateType> , IPlateTypeRepository
    {
        public PlateTypeRepository(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
