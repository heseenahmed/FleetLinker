using Benzeny.Infra.Data;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.City;
using BenzenyMain.Domain.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BenzenyMain.Infra.Repository
{
    public class CityRepository : ICityRepository
    {
        private readonly ApplicationDbContext _context;

        public CityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<City>> GetCitiesByRegionIdAsync(Guid regionId)
        {
            if (regionId == Guid.Empty)
                throw new ArgumentException("Invalid region ID.", nameof(regionId));

            return await _context.Cities
                .AsNoTracking()
                .Where(x => x.RegionId == regionId)
                .ToListAsync();
        }
    }
}
